#nullable disable
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace APP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private IMongoDatabase database;
        private IManagerService service;
        private string dateToday;
        public ManagerController()
        {
            var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
            var client = new MongoClient(settings);
            database = client.GetDatabase("USI");

            service = new ManagerService();

            dateToday = DateTime.Now.ToString("yyyy-MM-dd");
        }

        // GET: api/Manager/rooms
        [HttpGet("rooms")]
        public async Task<List<Room>> GetRooms()
        {
            var rooms = database.GetCollection<Room>("Rooms");

            return await rooms.Find(item => true).ToListAsync();
        }

        // GET: api/Manager/drugs
        [HttpGet("drugs")]
        public async Task<List<Drug>> GetDrugs()
        {
            var drugs = database.GetCollection<Drug>("Drugs");

            return await drugs.Find(item => true).ToListAsync();
        }

        // GET: api/Manager/ingredients
        [HttpGet("ingredients")]
        public async Task<DrugIngredients> GetIngredients()
        {
            var drugIngredients = database.GetCollection<DrugIngredients>("DrugIngredients");

            return await drugIngredients.Find(item => true).FirstOrDefaultAsync();
        }

        // GET: api/Manager/polls
        [HttpGet("polls")]
        public async Task<Hospital> GetHospitalPolls()
        {
            var hospital = database.GetCollection<Hospital>("Hospital");

            return await hospital.Find(item => true).FirstOrDefaultAsync();
        }

        // GET: api/Manager/doctorpolls
        [HttpGet("doctorpolls")]
        public async Task<List<PollForDoctors>> GetDoctorPolls()
        {
            var employees = database.GetCollection<PollForDoctors>("Employees");

            return await employees.Find(item => item.Role == "doctor").ToListAsync();
        }

        // POST: api/Manager/rooms
        [HttpPost("rooms")]
        public async Task<IActionResult> CreateRoom(Room data)
        {
            var rooms = database.GetCollection<Room>("Rooms");

            // If room with that name already exists abort action
            if (!service.IsRoomNameValid(rooms, data))
            {
                return BadRequest();
            }

            await rooms.InsertOneAsync(data);

            return Ok();
        }

        // POST: api/Manager/renovations
        [HttpPost("renovations")]
        public async Task<IActionResult> CreateRenovation(Renovation data)
        {
            var medicalExaminations = database.GetCollection<Examination>("MedicalExaminations");
            var examinationsInRoom = await medicalExaminations.Find(item => item.RoomName == data.Room).ToListAsync();

            var renovations = database.GetCollection<Renovation>("Renovations");
            var renovationsOnRoom = await renovations.Find(item => item.Room == data.Room).ToListAsync();

            // Check if some examinations are already scheduled at that time and if so abort action 
            if (service.ExaminationScheduledAtThatTime(examinationsInRoom, data) || service.RenovationScheduledAtThatTime(renovationsOnRoom, data))
            {
                return BadRequest();
            }

            await renovations.InsertOneAsync(data);

            // If renovation starts now update data in DB
            if (dateToday == data.StartDate)
            {
                var rooms = database.GetCollection<Room>("Rooms");
                var update = Builders<Room>.Update.Set("inRenovation", true);
                await rooms.UpdateManyAsync(item => item.Name == data.Room, update);
            }

            return Ok();
        }

        // POST: api/Manager/renovationdevide
        [HttpPost("renovationdevide")]
        public async Task<IActionResult> DevideRenovation(Renovation data)
        {
            var medicalExaminations = database.GetCollection<Examination>("MedicalExaminations");
            var examinationsInRoom = await medicalExaminations.Find(item => item.RoomName == data.Room).ToListAsync();

            var renovations = database.GetCollection<Renovation>("Renovations");
            var renovationsOnRoom = await renovations.Find(item => item.Room == data.Room).ToListAsync();

            // Check if some examinations are already scheduled at that time and if so abort action 
            if (service.ExaminationScheduledAtThatTime(examinationsInRoom, data) || service.RenovationScheduledAtThatTime(renovationsOnRoom, data))
            {
                return BadRequest();
            }

            if (dateToday == data.StartDate)
            {
                var rooms = database.GetCollection<Room>("Rooms");
                var roomFilter = Builders<Room>.Filter.Eq("name", data.Room);
                var roomToBeDevided = await rooms.Find(roomFilter).FirstOrDefaultAsync();

                var equipmentForRoom1 = new List<Equipment>();
                var equipmentForRoom2 = new List<Equipment>();

                service.DevideEquipment(roomToBeDevided, equipmentForRoom1, equipmentForRoom2);

                var room1 = new Room(roomToBeDevided.Name + ".1", roomToBeDevided.Type, true, equipmentForRoom1);
                var room2 = new Room(roomToBeDevided.Name + ".2", roomToBeDevided.Type, true, equipmentForRoom2);

                await rooms.InsertOneAsync(room1);
                await rooms.InsertOneAsync(room2);
                await rooms.DeleteOneAsync(roomFilter);

                // Delete it also from everywhere elese
                var transfers = database.GetCollection<Transfer>("RelocationOfEquipment");
                await transfers.DeleteManyAsync(item => item.Room1 == data.Room | item.Room2 == data.Room);

                var updateRoom = Builders<Examination>.Update.Set("room", roomToBeDevided.Name + ".1");
                await medicalExaminations.UpdateManyAsync(item => item.RoomName == data.Room, updateRoom);

                await renovations.DeleteManyAsync(item => item.Room == data.Room);

                var renovation1 = new Renovation(data.Room + ".1", data.StartDate, data.EndDate, false, "simple");
                var renovation2 = new Renovation(data.Room + ".2", data.StartDate, data.EndDate, false, "simple");

                await renovations.InsertOneAsync(renovation1);
                await renovations.InsertOneAsync(renovation2);
            }
            else
            {
                await renovations.InsertOneAsync(data);
            }

            return Ok();
        }

        // POST: api/Manager/renovationmerge
        [HttpPost("renovationmerge")]
        public async Task<IActionResult> MergeRenovation(Renovation data)
        {
            var medicalExaminations = database.GetCollection<Examination>("MedicalExaminations");
            var examinationsInRooms = await medicalExaminations.Find(item => item.RoomName == data.Room | item.RoomName == data.Room2).ToListAsync();

            var renovations = database.GetCollection<Renovation>("Renovations");
            var renovationsOnRoom = await renovations.Find(item => item.Room == data.Room).ToListAsync();

            // Check if some examinations are already scheduled at that time and if so abort action 
            if (service.ExaminationScheduledAtThatTime(examinationsInRooms, data) || service.RenovationScheduledAtThatTime(renovationsOnRoom, data))
            {
                return BadRequest();
            }

            if (dateToday == data.StartDate)
            {
                var rooms = database.GetCollection<Room>("Rooms");
                var room1 = await rooms.Find(item => item.Name == data.Room).FirstOrDefaultAsync();
                var room2 = await rooms.Find(item => item.Name == data.Room2).FirstOrDefaultAsync();

                var equipmentOfNewRoom = service.GetMergedRoomsEquipment(room1, room2);

                var newRoom = new Room(room1.Name, room1.Type, false, equipmentOfNewRoom);

                await rooms.DeleteOneAsync(item => item.Name == room1.Name);
                await rooms.DeleteOneAsync(item => item.Name == room2.Name);
                await rooms.InsertOneAsync(newRoom);

                // Delete them also from everywhere elese
                var transfers = database.GetCollection<Transfer>("RelocationOfEquipment");
                await transfers.DeleteManyAsync(item => item.Room1 == room1.Name | item.Room1 == room2.Name | item.Room2 == room1.Name | item.Room2 == room2.Name);

                var updateRoom = Builders<Examination>.Update.Set("room", room1.Name);
                await medicalExaminations.UpdateManyAsync(item => item.RoomName == room2.Name, updateRoom);

                await renovations.DeleteManyAsync(item => item.Room == room1.Name | item.Room == room2.Name);

                var renovation = new Renovation(room1.Name, data.StartDate, data.EndDate, false, "simple");

                await renovations.InsertOneAsync(renovation);
            }
            else
            {
                await renovations.InsertOneAsync(data);
            }

            return Ok();
        }

        // POST: api/Manager/transfer
        [HttpPost("transfer")]
        public async Task<IActionResult> CreateTransfer(Transfer data)
        {
            if (dateToday == data.Date)
            {
                var rooms = database.GetCollection<Room>("Rooms");
                foreach (var item in data.Equipment)
                {
                    var filter1 = Builders<Room>.Filter.Eq("name", data.Room1) & Builders<Room>.Filter.Eq("equipment.name", item.Name);
                    var filter2 = Builders<Room>.Filter.Eq("name", data.Room2) & Builders<Room>.Filter.Eq("equipment.name", item.Name);

                    var update1 = Builders<Room>.Update.Inc("equipment.$.quantity", -1 * item.Quantity);
                    await rooms.UpdateOneAsync(filter1, update1);

                    // Increment quantity of selected equipment in room2 or add it if it is new
                    if (await rooms.Find(filter2).FirstOrDefaultAsync() != null)
                    {
                        var update2 = Builders<Room>.Update.Inc("equipment.$.quantity", item.Quantity);
                        await rooms.UpdateOneAsync(filter2, update2);
                    }
                    else
                    {
                        var update2 = Builders<Room>.Update.Push("equipment", item);
                        await rooms.UpdateOneAsync(Builders<Room>.Filter.Eq("name", data.Room2), update2);
                    }
                }
                data.Done = true;
            }

            var transfers = database.GetCollection<Transfer>("RelocationOfEquipment");
            await transfers.InsertOneAsync(data);

            return Ok();
        }

        // POST: api/Manager/drugs
        [HttpPost("drugs")]
        public async Task<IActionResult> CreateDrug(Drug data)
        {
            var drugs = database.GetCollection<Drug>("Drugs");

            // If drug with that name already exists abort action
            if (!service.IsDrugNameValid(drugs, data))
            {
                return BadRequest();
            }

            await drugs.InsertOneAsync(data);

            return Ok();
        }

        // POST: api/Manager/ingredients
        [HttpPost("ingredients")]
        public async Task<IActionResult> CreateIngredinet(Dictionary<string, string> data)
        {
            var drugIngredients = database.GetCollection<DrugIngredients>("DrugIngredients");
            var allIngredients = await drugIngredients.Find(item => true).FirstOrDefaultAsync();

            // If drug with that name already exists abort action
            if (allIngredients.Ingredients.Contains(data["name"]))
            {
                return BadRequest();
            }

            var update = Builders<DrugIngredients>.Update.Push("ingredients", data["name"]);
            await drugIngredients.UpdateManyAsync(item => true, update);

            return Ok();
        }


        // PUT: api/Manager/rooms/1
        [HttpPut("rooms/{id}")]
        public async Task<IActionResult> UpdateRoom(string id, Room data)
        {
            var rooms = database.GetCollection<Room>("Rooms");

            if (!service.IsRoomNameValid(rooms, data))
            {
                return BadRequest();
            }

            // Update informations about the room wherever it is located
            var filter = Builders<Room>.Filter.Eq("name", id);
            var update = Builders<Room>.Update.Set("type", data.Type);
            await rooms.UpdateOneAsync(filter, update);
            update = Builders<Room>.Update.Set("name", data.Name);
            await rooms.UpdateOneAsync(filter, update);

            var renovations = database.GetCollection<Renovation>("Renovations");
            var updateRenovations = Builders<Renovation>.Update.Set("room", data.Name);
            await renovations.UpdateManyAsync(item => item.Room == id, updateRenovations);

            var examinations = database.GetCollection<Examination>("MedicalExaminations");
            var updateExaminations = Builders<Examination>.Update.Set("room", data.Name);
            await examinations.UpdateManyAsync(item => item.RoomName == id, updateExaminations);

            var transfers = database.GetCollection<Transfer>("RelocationOfEquipment");
            var updateTransfer = Builders<Transfer>.Update.Set("room1", data.Name);
            await transfers.UpdateManyAsync(item => item.Room1 == id | item.Room2 == id, updateTransfer);

            return Ok();
        }

        // PUT: api/Manager/drugs/1
        [HttpPut("drugs/{id}")]
        public async Task<IActionResult> UpdateDrug(string id, Drug data)
        {
            var drugs = database.GetCollection<Drug>("Drugs");

            if (!service.IsDrugNameValid(drugs, data))
            {
                return BadRequest();
            }

            var filter = Builders<Drug>.Filter.Eq("name", id);
            var update = Builders<Drug>.Update.Set("ingredients", data.Ingredients);
            await drugs.UpdateOneAsync(filter, update);
            update = Builders<Drug>.Update.Set("name", data.Name);
            await drugs.UpdateOneAsync(filter, update);
            update = Builders<Drug>.Update.Set("status", data.Status);
            await drugs.UpdateOneAsync(filter, update);

            return Ok();
        }

        // PUT: api/Manager/ingredients
        [HttpPut("ingredients/{id}")]
        public async Task<IActionResult> UpdateIngredinet(string id, Dictionary<string, string> data)
        {
            var drugIngredients = database.GetCollection<DrugIngredients>("DrugIngredients");
            var allIngredients = await drugIngredients.Find(item => true).FirstOrDefaultAsync();

            if (!service.IsIngredientNameValid(allIngredients, data))
            {
                return BadRequest();
            }

            allIngredients.Ingredients.Remove(id);
            allIngredients.Ingredients.Add(data["name"]);

            var update = Builders<DrugIngredients>.Update.Set("ingredients", allIngredients.Ingredients);
            await drugIngredients.UpdateManyAsync(item => true, update);

            return Ok();
        }

        // DELETE: api/Manager/rooms/1
        [HttpDelete("rooms/{id}")]
        public async Task<IActionResult> DeleteRoom(string id)
        {
            var rooms = database.GetCollection<Room>("Rooms");
            await rooms.DeleteOneAsync(item => item.Name == id);

            var renovations = database.GetCollection<Renovation>("Renovations");
            await renovations.DeleteManyAsync(item => item.Room == id);

            var transfers = database.GetCollection<Transfer>("RelocationOfEquipment");
            await transfers.DeleteManyAsync(item => item.Room1 == id | item.Room2 == id);

            return Ok();
        }

        // DELETE: api/Manager/drugs/1
        [HttpDelete("drugs/{id}")]
        public async Task<IActionResult> DeleteDrug(string id)
        {
            var drugs = database.GetCollection<Drug>("Drugs");
            await drugs.DeleteOneAsync(item => item.Name == id);

            return Ok();
        }

        // DELETE: api/Manager/ingredients/1
        [HttpDelete("ingredients/{id}")]
        public async Task<IActionResult> DeleteIngredient(string id)
        {
            var drugIngredients = database.GetCollection<DrugIngredients>("DrugIngredients");
            var allIngredients = await drugIngredients.Find(item => true).FirstOrDefaultAsync();

            allIngredients.Ingredients.Remove(id);

            var update = Builders<DrugIngredients>.Update.Set("ingredients", allIngredients.Ingredients);
            await drugIngredients.UpdateManyAsync(item => true, update);

            return Ok();
        }
    }
}
