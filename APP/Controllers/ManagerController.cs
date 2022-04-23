#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.AspNetCore.Cors;

namespace APP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private IMongoDatabase database;
        public ManagerController()
        {
            var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
            var client = new MongoClient(settings);
            database = client.GetDatabase("USI");
        }

        // GET: api/Manager/rooms
        [HttpGet("rooms")]
        public async Task<List<Room>> GetRooms()
        {
            var collection = database.GetCollection<Room>("Rooms");

            return collection.Find(item => true).ToList();
        }

        // GET: api/Manager/drugs
        [HttpGet("drugs")]
        public async Task<List<Drug>> GetDrugs()
        {
            var collection = database.GetCollection<Drug>("Drugs");

            return collection.Find(item => true).ToList();
        }

        // GET: api/Manager/ingredients
        [HttpGet("ingredients")]
        public async Task<DrugIngredients> GetIngredients()
        {
            var collection = database.GetCollection<DrugIngredients>("DrugIngredients");

            return collection.Find(item => true).First();
        }

        // POST: api/Manager/rooms
        [HttpPost("rooms")]
        public async Task<IActionResult> CreateRoom(Room data)
        {
            var collection = database.GetCollection<Room>("Rooms");

            // If room with that name already exists abort action
            if (collection.Find(item => item.name == data.name).ToList().Count != 0)
            {
                return BadRequest();
            }

            collection.InsertOne(data);

            return Ok();
        }

        // POST: api/Manager/renovations
        [HttpPost("renovations")]
        public async Task<IActionResult> CreateRenovation(Renovation data)
        {
            var collection = database.GetCollection<Examination>("MedicalExaminations");
            var examinationsInRoom = collection.Find(item => item.roomName == data.room).ToList();

            // Check if some examinations are already scheduled at that time and if so abort action 
            foreach (var el in examinationsInRoom)
            {
                if (DateTime.Parse(el.dateAndTimeOfExamination) >= DateTime.Parse(data.startDate) && DateTime.Parse(el.dateAndTimeOfExamination) <= DateTime.Parse(data.endDate))
                {
                    return BadRequest();
                }
            }

            var collectionRenovations = database.GetCollection<Renovation>("Renovations");

            collectionRenovations.InsertOne(data);

            string date = DateTime.UtcNow.ToString("yyyy-MM-dd");

            // If renovation starts now update data in DB
            if (date == data.startDate)
            {
                var collectionRooms = database.GetCollection<Room>("Rooms");
                var update = Builders<Room>.Update.Set("inRenovation", true);
                collectionRooms.UpdateMany(item => item.name == data.room, update);
            }

            return Ok();
        }

        // POST: api/Manager/renovationdevide
        [HttpPost("renovationdevide")]
        public async Task<IActionResult> DevideRenovation(Renovation data)
        {
            var collection = database.GetCollection<Examination>("MedicalExaminations");
            var examinationsInRoom = collection.Find(item => item.roomName == data.room).ToList();

            // Check if some examinations are already scheduled at that time and if so abort action 
            foreach (var el in examinationsInRoom)
            {
                if (DateTime.Parse(el.dateAndTimeOfExamination) >= DateTime.Parse(data.startDate) && DateTime.Parse(el.dateAndTimeOfExamination) <= DateTime.Parse(data.endDate))
                {
                    return BadRequest();
                }
            }

            var collectionRooms = database.GetCollection<Room>("Rooms");
            var roomFilter = Builders<Room>.Filter.Eq("name", data.room);
            var roomToBeDevided = collectionRooms.Find(roomFilter).FirstOrDefault();

            List<Equipment> equipmentForRoom1 = new List<Equipment>();
            List<Equipment> equipmentForRoom2 = new List<Equipment>();

            foreach (var el in roomToBeDevided.equipment)
            {
                equipmentForRoom1.Add(new Equipment(el.name, el.type, el.quantity / 2));
                equipmentForRoom2.Add(new Equipment(el.name, el.type, el.quantity - el.quantity / 2));
            }

            var room1 = new Room(roomToBeDevided.name + ".1", roomToBeDevided.type, false, equipmentForRoom1);
            var room2 = new Room(roomToBeDevided.name + ".2", roomToBeDevided.type, false, equipmentForRoom2);

            collectionRooms.InsertOne(room1);
            collectionRooms.InsertOne(room2);
            collectionRooms.DeleteOne(roomFilter);

            // Delete it also from everywhere elese
            var collectionTransfer = database.GetCollection<Transfer>("RelocationOfEquipment");
            collectionTransfer.DeleteMany(item => item.room1 == data.room | item.room2 == data.room);

            var updateRoom = Builders<Examination>.Update.Set("room", roomToBeDevided.name + ".1");
            collection.UpdateMany(item => item.roomName == data.room, updateRoom);

            var collectionRenovations = database.GetCollection<Renovation>("Renovations");
            collectionRenovations.DeleteMany(item => item.room == data.room);

            var renovation1 = new Renovation(data.room + ".1", data.startDate, data.endDate);
            var renovation2 = new Renovation(data.room + ".2", data.startDate, data.endDate);

            collectionRenovations.InsertOne(renovation1);
            collectionRenovations.InsertOne(renovation2);

            string date = DateTime.UtcNow.ToString("yyyy-MM-dd");

            if (date == data.startDate)
            {
                var update = Builders<Room>.Update.Set("inRenovation", true);
                collectionRooms.UpdateMany(item => item.name == room1.name | item.name == room2.name, update);
            }

            return Ok();
        }

        // POST: api/Manager/renovationmerge
        [HttpPost("renovationmerge")]
        public async Task<IActionResult> MergeRenovation(RenovationMerge data)
        {
            var collection = database.GetCollection<Examination>("MedicalExaminations");
            var examinationsInRooms = collection.Find(item => item.roomName == data.room1 | item.roomName == data.room2).ToList();

            // Check if some examinations are already scheduled at that time and if so abort action 
            foreach (var el in examinationsInRooms)
            {
                if (DateTime.Parse(el.dateAndTimeOfExamination) >= DateTime.Parse(data.startDate) && DateTime.Parse(el.dateAndTimeOfExamination) <= DateTime.Parse(data.endDate))
                {
                    return BadRequest();
                }
            }

            var collectionRooms = database.GetCollection<Room>("Rooms");
            var room1 = collectionRooms.Find(item => item.name == data.room1).FirstOrDefault();
            var room2 = collectionRooms.Find(item => item.name == data.room2).FirstOrDefault();

            List<Equipment> equipmentOfNewRoom = new List<Equipment>();

            foreach (var el in room1.equipment)
            {
                equipmentOfNewRoom.Add(el);
            }

            foreach (var el in room2.equipment)
            {
                int index = equipmentOfNewRoom.FindIndex(item => item.name == el.name);
                if (index != -1)
                {
                    equipmentOfNewRoom[index].quantity += el.quantity;
                }
                else
                {
                    equipmentOfNewRoom.Add(el);
                }
            }

            var newRoom = new Room(room1.name, room1.type, false, equipmentOfNewRoom);

            collectionRooms.DeleteOne(item => item.name == room1.name);
            collectionRooms.DeleteOne(item => item.name == room2.name);
            collectionRooms.InsertOne(newRoom);

            // Delete them also from everywhere elese
            var collectionTransfer = database.GetCollection<Transfer>("RelocationOfEquipment");
            collectionTransfer.DeleteMany(item => item.room1 == room1.name | item.room1 == room2.name | item.room2 == room1.name | item.room2 == room2.name);

            var updateRoom = Builders<Examination>.Update.Set("room", room1.name);
            collection.UpdateMany(item => item.roomName == room2.name, updateRoom);

            var collectionRenovations = database.GetCollection<Renovation>("Renovations");
            collectionRenovations.DeleteMany(item => item.room == room1.name | item.room == room2.name);

            var renovation = new Renovation(room1.name, data.startDate, data.endDate);

            collectionRenovations.InsertOne(renovation);

            string date = DateTime.UtcNow.ToString("yyyy-MM-dd");

            if (date == data.startDate)
            {
                var update = Builders<Room>.Update.Set("inRenovation", true);
                collectionRooms.UpdateMany(item => item.name == room1.name, update);
            }

            return Ok();
        }

        // POST: api/Manager/transfer
        [HttpPost("transfer")]
        public async Task<IActionResult> CreateTransfer(Transfer data)
        {
            var collection = database.GetCollection<Room>("Rooms");
            foreach (var item in data.equipment)
            {
                var filter1 = Builders<Room>.Filter.Eq("name", data.room1) & Builders<Room>.Filter.Eq("equipment.name", item.name);
                var filter2 = Builders<Room>.Filter.Eq("name", data.room2) & Builders<Room>.Filter.Eq("equipment.name", item.name);

                var update1 = Builders<Room>.Update.Inc("equipment.$.quantity", -1 * item.quantity);
                collection.UpdateOne(filter1, update1);

                if (collection.Find(filter2).ToList().Count != 0)
                {
                    var update2 = Builders<Room>.Update.Inc("equipment.$.quantity", item.quantity);
                    collection.UpdateOne(filter2, update2);
                }
                else
                {
                    var update2 = Builders<Room>.Update.Push("equipment", item);
                    collection.UpdateOne(Builders<Room>.Filter.Eq("name", data.room2), update2);
                }
            }

            var collectionTransfer = database.GetCollection<Transfer>("RelocationOfEquipment");
            collectionTransfer.InsertOne(data);

            return Ok();
        }

        // POST: api/Manager/drugs
        [HttpPost("drugs")]
        public async Task<IActionResult> CreateDrug(Drug data)
        {
            var collection = database.GetCollection<Drug>("Drugs");

            // If drug with that name already exists abort action
            if (collection.Find(item => item.name == data.name).ToList().Count != 0)
            {
                return BadRequest();
            }

            collection.InsertOne(data);

            return Ok();
        }

        // POST: api/Manager/ingredients
        [HttpPost("ingredients")]
        public async Task<IActionResult> CreateIngredinet(Dictionary<string, string> data)
        {
            var collection = database.GetCollection<DrugIngredients>("DrugIngredients");
            var result = collection.Find(item => true).First();

            // If drug with that name already exists abort action
            if (result.ingredients.Contains(data["name"]))
            {
                return BadRequest();
            }

            var update = Builders<DrugIngredients>.Update.Push("ingredients", data["name"]);
            collection.UpdateMany(item => true, update);

            return Ok();
        }


        // PUT: api/Manager/rooms/1
        [HttpPut("rooms/{id}")]
        public async Task<IActionResult> UpdateRoom(string id, Room data)
        {
            var collection = database.GetCollection<Room>("Rooms");

            if (data.name != id && collection.Find(item => item.name == data.name).ToList().Count != 0)
            {
                return BadRequest();
            }

            // Update informations about the room wherever it is located
            var filter = Builders<Room>.Filter.Eq("name", id);
            var update = Builders<Room>.Update.Set("type", data.type);
            collection.UpdateOne(filter, update);
            update = Builders<Room>.Update.Set("name", data.name);
            collection.UpdateOne(filter, update);

            var collectionRenovations = database.GetCollection<Renovation>("Renovations");
            var updateRenovations = Builders<Renovation>.Update.Set("room", data.name);
            collectionRenovations.UpdateMany(item => item.room == id, updateRenovations);

            var collectionExaminations = database.GetCollection<Examination>("MedicalExaminations");
            var updateExaminations = Builders<Examination>.Update.Set("room", data.name);
            collectionExaminations.UpdateMany(item => item.roomName == id, updateExaminations);

            var collectionTransfer = database.GetCollection<Transfer>("RelocationOfEquipment");
            var updateTransfer = Builders<Transfer>.Update.Set("room1", data.name);
            collectionTransfer.UpdateMany(item => item.room1 == id | item.room2 == id, updateTransfer);

            return Ok();
        }

        // PUT: api/Manager/drugs/1
        [HttpPut("drugs/{id}")]
        public async Task<IActionResult> UpdateDrug(string id, Drug data)
        {
            var collection = database.GetCollection<Drug>("Drugs");

            if (data.name != id && collection.Find(item => item.name == data.name).ToList().Count != 0)
            {
                return BadRequest();
            }

            var filter = Builders<Drug>.Filter.Eq("name", id);
            var update = Builders<Drug>.Update.Set("ingredients", data.ingredients);
            collection.UpdateOne(filter, update);
            update = Builders<Drug>.Update.Set("name", data.name);
            collection.UpdateOne(filter, update);
            update = Builders<Drug>.Update.Set("status", data.status);
            collection.UpdateOne(filter, update);

            return Ok();
        }

        // PUT: api/Manager/ingredients
        [HttpPut("ingredients/{id}")]
        public async Task<IActionResult> UpdateIngredinet(string id, Dictionary<string, string> data)
        {
            var collection = database.GetCollection<DrugIngredients>("DrugIngredients");
            var result = collection.Find(item => true).First();

            if (id != data["name"] && result.ingredients.Contains(data["name"]))
            {
                return BadRequest();
            }

            result.ingredients.Remove(id);
            result.ingredients.Add(data["name"]);

            var update = Builders<DrugIngredients>.Update.Set("ingredients", result.ingredients);
            collection.UpdateMany(item => true, update);

            return Ok();
        }

        // DELETE: api/Manager/rooms/1
        [HttpDelete("rooms/{id}")]
        public async Task<IActionResult> DeleteRoom(string id)
        {
            var collection = database.GetCollection<Room>("Rooms");
            collection.DeleteOne(item => item.name == id);

            var collectionRenovations = database.GetCollection<Renovation>("Renovations");
            collectionRenovations.DeleteMany(item => item.room == id);

            var collectionTransfer = database.GetCollection<Transfer>("RelocationOfEquipment");
            collectionTransfer.DeleteMany(item => item.room1 == id | item.room2 == id);

            return Ok();
        }

        // DELETE: api/Manager/drugs/1
        [HttpDelete("drugs/{id}")]
        public async Task<IActionResult> DeleteDrug(string id)
        {
            var collection = database.GetCollection<Drug>("Drugs");
            collection.DeleteOne(item => item.name == id);

            return Ok();
        }

        // DELETE: api/Manager/ingredients/1
        [HttpDelete("ingredients/{id}")]
        public async Task<IActionResult> DeleteIngredient(string id)
        {
            var collection = database.GetCollection<DrugIngredients>("DrugIngredients");
            var result = collection.Find(item => true).First();

            result.ingredients.Remove(id);

            var update = Builders<DrugIngredients>.Update.Set("ingredients", result.ingredients);
            collection.UpdateMany(item => true, update);

            return Ok();
        }
    }
}
