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
            var results = collection.Find(item => item.roomName == data.room).ToList();

            // Check if some examinations are already scheduled at that time and if so abort action 
            foreach (var el in results)
            {
                if (DateTime.Parse(el.dateAndTimeOfExamination) >= DateTime.Parse(data.startDate) && DateTime.Parse(el.dateAndTimeOfExamination) <= DateTime.Parse(data.endDate))
                {
                    return BadRequest();
                }
            }

            var collection2 = database.GetCollection<Renovation>("Renovations");

            collection2.InsertOne(data);

            string date = DateTime.UtcNow.ToString("yyyy-MM-dd");

            // If renovation starts now update data in DB
            if (date == data.startDate)
            {
                var collection3 = database.GetCollection<Room>("Rooms");
                var filter = Builders<Room>.Filter.Eq("name", data.room);
                var update = Builders<Room>.Update.Set("inRenovation", true);
                collection3.UpdateMany(filter, update);
            }

            return Ok();
        }

        // POST: api/Manager/renovationdevide
        [HttpPost("renovationdevide")]
        public async Task<IActionResult> DevideRenovation(Renovation data)
        {
            var collection = database.GetCollection<Examination>("MedicalExaminations");
            var results = collection.Find(item => item.roomName == data.room).ToList();

            // Check if some examinations are already scheduled at that time and if so abort action 
            foreach (var el in results)
            {
                if (DateTime.Parse(el.dateAndTimeOfExamination) >= DateTime.Parse(data.startDate) && DateTime.Parse(el.dateAndTimeOfExamination) <= DateTime.Parse(data.endDate))
                {
                    return BadRequest();
                }
            }

            var collection2 = database.GetCollection<Room>("Rooms");
            var result = collection2.Find(item => item.name == data.room).FirstOrDefault();

            List<Equipment> equipment1 = new List<Equipment>();
            List<Equipment> equipment2 = new List<Equipment>();

            foreach (var el in result.equipment)
            {
                equipment1.Add(new Equipment(el.name, el.type, el.quantity / 2));
                equipment2.Add(new Equipment(el.name, el.type, el.quantity - el.quantity / 2));
            }

            var room1 = new Room(result.name + ".1", result.type, false, equipment1);
            var room2 = new Room(result.name + ".2", result.type, false, equipment2);

            collection2.InsertOne(room1);
            collection2.InsertOne(room2);
            collection2.DeleteOne(item => item.name == data.room);

            // Delete it also from everywhere elese
            var collection3 = database.GetCollection<Transfer>("RelocationOfEquipment");
            collection3.DeleteMany(item => item.room1 == data.room | item.room2 == data.room);

            var filter4 = Builders<Examination>.Filter.Eq("room", data.room);
            var update4 = Builders<Examination>.Update.Set("room", result.name + ".1");
            collection.UpdateMany(filter4, update4);

            var collection5 = database.GetCollection<Renovation>("Renovations");
            collection5.DeleteMany(item => item.room == data.room);

            var renovation1 = new Renovation(data.room + ".1", data.startDate, data.endDate);
            var renovation2 = new Renovation(data.room + ".2", data.startDate, data.endDate);

            collection5.InsertOne(renovation1);
            collection5.InsertOne(renovation2);

            string date = DateTime.UtcNow.ToString("yyyy-MM-dd");

            if (date == data.startDate)
            {
                var filter = Builders<Room>.Filter.Eq("name", result.name + ".1") | Builders<Room>.Filter.Eq("name", result.name + ".2");
                var update = Builders<Room>.Update.Set("inRenovation", true);
                collection2.UpdateMany(filter, update);
            }

            return Ok();
        }

        // POST: api/Manager/renovationmerge
        [HttpPost("renovationmerge")]
        public async Task<IActionResult> MergeRenovation(RenovationMerge data)
        {
            var collection = database.GetCollection<Examination>("MedicalExaminations");
            var filter = Builders<Examination>.Filter.Eq("room", data.room1) | Builders<Examination>.Filter.Eq("room", data.room2);
            var results = collection.Find(filter).ToList();

            // Check if some examinations are already scheduled at that time and if so abort action 
            foreach (var el in results)
            {
                if (DateTime.Parse(el.dateAndTimeOfExamination) >= DateTime.Parse(data.startDate) && DateTime.Parse(el.dateAndTimeOfExamination) <= DateTime.Parse(data.endDate))
                {
                    return BadRequest();
                }
            }

            var collection2 = database.GetCollection<Room>("Rooms");
            var room1 = collection2.Find(item => item.name == data.room1).FirstOrDefault();
            var room2 = collection2.Find(item => item.name == data.room2).FirstOrDefault();

            List<Equipment> equipment = new List<Equipment>();

            foreach (var el in room1.equipment)
            {
                equipment.Add(el);
            }

            foreach (var el in room2.equipment)
            {
                int index = equipment.FindIndex(item => item.name == el.name);
                if (index != -1)
                {
                    equipment[index].quantity += el.quantity;
                }
                else
                {
                    equipment.Add(el);
                }
            }

            var newRoom = new Room(room1.name, room1.type, false, equipment);

            collection2.DeleteOne(item => item.name == room1.name);
            collection2.DeleteOne(item => item.name == room2.name);
            collection2.InsertOne(newRoom);

            // Delete them also from everywhere elese
            var collection3 = database.GetCollection<Transfer>("RelocationOfEquipment");
            collection3.DeleteMany(item => item.room1 == room1.name | item.room1 == room2.name | item.room2 == room1.name | item.room2 == room2.name);

            var filter4 = Builders<Examination>.Filter.Eq("room", room2.name);
            var update4 = Builders<Examination>.Update.Set("room", room1.name);
            collection.UpdateMany(filter4, update4);

            var collection5 = database.GetCollection<Renovation>("Renovations");
            collection5.DeleteMany(item => item.room == room1.name | item.room == room2.name);

            var renovation = new Renovation(room1.name, data.startDate, data.endDate);

            collection5.InsertOne(renovation);

            string date = DateTime.UtcNow.ToString("yyyy-MM-dd");

            if (date == data.startDate)
            {
                var filter2 = Builders<Room>.Filter.Eq("name", room1.name);
                var update2 = Builders<Room>.Update.Set("inRenovation", true);
                collection2.UpdateMany(filter2, update2);
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

            var collection2 = database.GetCollection<Transfer>("RelocationOfEquipment");
            collection2.InsertOne(data);

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
            var update = Builders<Room>.Update.Set("type", data.type);
            var filter = Builders<Room>.Filter.Eq("name", id);
            collection.UpdateOne(filter, update);
            update = Builders<Room>.Update.Set("name", data.name);
            collection.UpdateOne(filter, update);

            var collection2 = database.GetCollection<Renovation>("Renovations");
            var filter2 = Builders<Renovation>.Filter.Eq("room", id);
            var update2 = Builders<Renovation>.Update.Set("room", data.name);
            collection2.UpdateMany(filter2, update2);

            var collection3 = database.GetCollection<Examination>("MedicalExaminations");
            var filter3 = Builders<Examination>.Filter.Eq("room", id);
            var update3 = Builders<Examination>.Update.Set("room", data.name);
            collection3.UpdateMany(filter3, update3);

            var collection4 = database.GetCollection<Transfer>("RelocationOfEquipment");
            var filter4 = Builders<Transfer>.Filter.Eq("room1", id);
            var update4 = Builders<Transfer>.Update.Set("room1", data.name);
            collection4.UpdateMany(filter4, update4);
            filter4 = Builders<Transfer>.Filter.Eq("room2", id);
            update4 = Builders<Transfer>.Update.Set("room2", data.name);
            collection4.UpdateMany(filter4, update4);

            return Ok();
        }

        // DELETE: api/Manager/rooms/1
        [HttpDelete("rooms/{id}")]
        public async Task<IActionResult> DeleteRoom(string id)
        {
            var collection = database.GetCollection<Room>("Rooms");
            collection.DeleteOne(item => item.name == id);

            var collection2 = database.GetCollection<Renovation>("Renovations");
            collection2.DeleteMany(item => item.room == id);

            var collection3 = database.GetCollection<Transfer>("RelocationOfEquipment");
            collection3.DeleteMany(item => item.room1 == id | item.room2 == id);

            return Ok();
        }
    }
}
