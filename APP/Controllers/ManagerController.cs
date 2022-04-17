#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APP.Models;
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
        public IActionResult GetRooms()
        {
            var collection = database.GetCollection<BsonDocument>("Rooms");

            var filter = Builders<BsonDocument>.Filter.Empty;
            var results = collection.Find(filter).ToList();
            var dotNetObjList = results.ConvertAll(BsonTypeMapper.MapToDotNetValue);
            Response.StatusCode = StatusCodes.Status200OK;
            return new JsonResult(dotNetObjList);
        }

        // POST: api/Manager/rooms
        [HttpPost("rooms")]
        public async Task<IActionResult> CreateRoom(string id, Data data)
        {
            var collection = database.GetCollection<BsonDocument>("Rooms");

            // If room with that name already exists abort action
            if (collection.Find(Builders<BsonDocument>.Filter.Eq("name", data.name)).ToList().Count != 0)
            {
                return BadRequest();
            }

            var document = new BsonDocument
            {
                { "type", data.type },
                { "name", data.name},
                {"inRenovation", false},
                { "equipment", new BsonDocument{}},
            };

            collection.InsertOne(document);

            return Ok();
        }

        // POST: api/Manager/renovations/1&t&t
        [HttpPost("renovations/{id}&{from}&{to}")]
        public async Task<IActionResult> CreateRenovation(string id, string from, string to)
        {

            var collection = database.GetCollection<BsonDocument>("MedicalExaminations");
            var filter = Builders<BsonDocument>.Filter.Eq("room", id);
            var results = collection.Find(filter).ToList();

            // Check if some examinations are already scheduled at that time and if so abort action 
            foreach (var el in results)
            {
                if (el["date"] >= from && el["date"] <= to)
                {
                    return BadRequest();
                }
            }


            collection = database.GetCollection<BsonDocument>("Renovations");

            var document = new BsonDocument
            {
                { "room", id },
                { "startDate", from },
                { "endDate", to }
            };

            collection.InsertOne(document);

            string date = DateTime.UtcNow.ToString("yyyy-MM-dd");

            // If renovation starts now update data in DB
            if (date == from)
            {
                collection = database.GetCollection<BsonDocument>("Rooms");
                filter = Builders<BsonDocument>.Filter.Eq("name", id);
                var update = Builders<BsonDocument>.Update.Set("inRenovation", true);
                collection.UpdateMany(filter, update);
            }

            return Ok();
        }

        // POST: api/Manager/renovationdevide/1&t&t
        [HttpPost("renovationdevide/{id}&{from}&{to}")]
        public async Task<IActionResult> DevideRenovation(string id, string from, string to)
        {

            var collection = database.GetCollection<BsonDocument>("MedicalExaminations");
            var filter = Builders<BsonDocument>.Filter.Eq("room", id);
            var results = collection.Find(filter).ToList();

            foreach (var el in results)
            {
                if (el["date"] >= from && el["date"] <= to)
                {
                    return BadRequest();
                }
            }

            collection = database.GetCollection<BsonDocument>("Rooms");
            filter = Builders<BsonDocument>.Filter.Eq("name", id);
            var result = collection.Find(filter).FirstOrDefault();

            BsonDocument equipment1 = new BsonDocument();
            BsonDocument equipment2 = new BsonDocument();

            BsonDocument equipment = (BsonDocument)result["equipment"];
            foreach (var el in equipment.ToDictionary())
            {
                var e1 = new BsonDocument{{el.Key,new BsonDocument{
                    {"type", equipment[el.Key]["type"]},
                    {"quantity", equipment[el.Key]["quantity"].ToInt32()/2}
                }}};
                var e2 = new BsonDocument{{el.Key,new BsonDocument{
                    {"type", equipment[el.Key]["type"]},
                    {"quantity", equipment[el.Key]["quantity"].ToInt32() - equipment[el.Key]["quantity"].ToInt32()/2}
                }}};
                equipment1.AddRange(e1);
                equipment2.AddRange(e2);
            }

            var room1 = new BsonDocument
            {
                { "type", result["type"] },
                { "name", result["name"] + ".1" },
                { "inRenovation", false },
                {"equipment", equipment1}
            };
            var room2 = new BsonDocument
            {
                { "type", result["type"] },
                { "name", result["name"] + ".2" },
                { "inRenovation", false },
                {"equipment", equipment2}
            };

            collection.InsertOne(room1);
            collection.InsertOne(room2);
            collection.DeleteOne(filter);

            // Delete it also from everywhere elese
            collection = database.GetCollection<BsonDocument>("RelocationOfEquipment");
            filter = Builders<BsonDocument>.Filter.Eq("from.room", id) | Builders<BsonDocument>.Filter.Eq("to.room", id);
            collection.DeleteMany(filter);

            collection = database.GetCollection<BsonDocument>("MedicalExaminations");
            filter = Builders<BsonDocument>.Filter.Eq("room", id);
            var update = Builders<BsonDocument>.Update.Set("room", result["name"] + ".1");
            collection.UpdateMany(filter, update);

            collection = database.GetCollection<BsonDocument>("Renovations");
            filter = Builders<BsonDocument>.Filter.Eq("room", id);
            collection.DeleteMany(filter);

            var renovation1 = new BsonDocument
            {
                { "room", result["name"] + ".1" },
                { "startDate", from },
                { "endDate", to }
            };
            var renovation2 = new BsonDocument
            {
                { "room", result["name"] + ".2" },
                { "startDate", from },
                { "endDate", to }
            };

            collection.InsertOne(renovation1);
            collection.InsertOne(renovation2);

            string date = DateTime.UtcNow.ToString("yyyy-MM-dd");

            if (date == from)
            {
                collection = database.GetCollection<BsonDocument>("Rooms");
                filter = Builders<BsonDocument>.Filter.Eq("name", result["name"] + ".1") | Builders<BsonDocument>.Filter.Eq("name", result["name"] + ".2");
                update = Builders<BsonDocument>.Update.Set("inRenovation", true);
                collection.UpdateMany(filter, update);
            }

            return Ok();
        }

        // POST: api/Manager/renovationmerge/1&2&t&t
        [HttpPost("renovationmerge/{id1}&{id2}&{from}&{to}")]
        public async Task<IActionResult> MergeRenovation(string id1, string id2, string from, string to)
        {

            var collection = database.GetCollection<BsonDocument>("MedicalExaminations");
            var filter = Builders<BsonDocument>.Filter.Eq("room", id1) | Builders<BsonDocument>.Filter.Eq("room", id2);
            var results = collection.Find(filter).ToList();

            foreach (var el in results)
            {
                if (el["date"] >= from && el["date"] <= to)
                {
                    return BadRequest();
                }
            }
            collection = database.GetCollection<BsonDocument>("Rooms");
            var filter1 = Builders<BsonDocument>.Filter.Eq("name", id1);
            var room1 = collection.Find(filter1).FirstOrDefault();
            var filter2 = Builders<BsonDocument>.Filter.Eq("name", id2);
            var room2 = collection.Find(filter2).FirstOrDefault();

            BsonDocument newEquipment = new BsonDocument();

            BsonDocument equipment1 = (BsonDocument)room1["equipment"];
            BsonDocument equipment2 = (BsonDocument)room2["equipment"];
            foreach (var el in equipment1.ToDictionary())
            {
                var e1 = new BsonDocument{{el.Key,new BsonDocument{
                    {"type", equipment1[el.Key]["type"]},
                    {"quantity", equipment1[el.Key]["quantity"]}
                }}};
                newEquipment.AddRange(e1);
            }
            foreach (var el in equipment2.ToDictionary())
            {
                if (newEquipment.Contains(el.Key))
                {
                    newEquipment[el.Key]["quantity"] = newEquipment[el.Key]["quantity"].ToInt32() + equipment2[el.Key]["quantity"].ToInt32();
                }
                else
                {
                    var e1 = new BsonDocument{{el.Key,new BsonDocument{
                    {"type", equipment1[el.Key]["type"]},
                    {"quantity", equipment1[el.Key]["quantity"]}
                }}};
                    newEquipment.AddRange(e1);
                }
            }

            var newRoom = new BsonDocument
            {
                { "type", room1["type"] },
                { "name", room1["name"] },
                { "inRenovation", false },
                {"equipment", newEquipment}
            };

            collection.DeleteOne(filter1);
            collection.DeleteOne(filter2);
            collection.InsertOne(newRoom);

            // Delete them also from everywhere elese
            collection = database.GetCollection<BsonDocument>("RelocationOfEquipment");
            filter = Builders<BsonDocument>.Filter.Eq("from.room", id1) | Builders<BsonDocument>.Filter.Eq("to.room", id1) | Builders<BsonDocument>.Filter.Eq("from.room", id2) | Builders<BsonDocument>.Filter.Eq("to.room", id2);
            collection.DeleteMany(filter);

            collection = database.GetCollection<BsonDocument>("MedicalExaminations");
            filter = Builders<BsonDocument>.Filter.Eq("room", id2);
            var update = Builders<BsonDocument>.Update.Set("room", id1);
            collection.UpdateMany(filter, update);

            collection = database.GetCollection<BsonDocument>("Renovations");
            filter = Builders<BsonDocument>.Filter.Eq("room", id1) | Builders<BsonDocument>.Filter.Eq("room", id2);
            collection.DeleteMany(filter);

            var renovation = new BsonDocument
            {
                { "room", room1["name"] },
                { "startDate", from },
                { "endDate", to }
            };

            collection.InsertOne(renovation);

            string date = DateTime.UtcNow.ToString("yyyy-MM-dd");

            if (date == from)
            {
                collection = database.GetCollection<BsonDocument>("Rooms");
                filter = Builders<BsonDocument>.Filter.Eq("name", room1["name"]);
                update = Builders<BsonDocument>.Update.Set("inRenovation", true);
                collection.UpdateMany(filter, update);
            }

            return Ok();
        }

        // PUT: api/Manager/rooms/1
        [HttpPut("rooms/{id}")]
        public async Task<IActionResult> UpdateRoom(string id, Data data)
        {
            var collection = database.GetCollection<BsonDocument>("Rooms");

            var filter = Builders<BsonDocument>.Filter.Eq("name", id);

            if (data.name != id && collection.Find(Builders<BsonDocument>.Filter.Eq("name", data.name)).ToList().Count != 0)
            {
                return BadRequest();
            }

            // Update informations about the room wherever it is located
            var update = Builders<BsonDocument>.Update.Set("type", data.type);
            collection.UpdateOne(filter, update);
            update = Builders<BsonDocument>.Update.Set("name", data.name);
            collection.UpdateOne(filter, update);

            collection = database.GetCollection<BsonDocument>("Renovations");
            filter = Builders<BsonDocument>.Filter.Eq("room", id);
            update = Builders<BsonDocument>.Update.Set("room", data.name);
            collection.UpdateMany(filter, update);

            collection = database.GetCollection<BsonDocument>("MedicalExaminations");
            filter = Builders<BsonDocument>.Filter.Eq("room", id);
            update = Builders<BsonDocument>.Update.Set("room", data.name);
            collection.UpdateMany(filter, update);

            collection = database.GetCollection<BsonDocument>("RelocationOfEquipment");
            filter = Builders<BsonDocument>.Filter.Eq("from.room", id);
            update = Builders<BsonDocument>.Update.Set("from.room", data.name);
            collection.UpdateMany(filter, update);
            filter = Builders<BsonDocument>.Filter.Eq("to.room", id);
            update = Builders<BsonDocument>.Update.Set("to.room", data.name);
            collection.UpdateMany(filter, update);

            return Ok();
        }

        // DELETE: api/Manager/rooms/1
        [HttpDelete("rooms/{id}")]
        public async Task<IActionResult> DeleteRoom(string id)
        {
            var collection = database.GetCollection<BsonDocument>("Rooms");
            var filter = Builders<BsonDocument>.Filter.Eq("name", id);
            collection.DeleteOne(filter);

            collection = database.GetCollection<BsonDocument>("Renovations");
            filter = Builders<BsonDocument>.Filter.Eq("room", id);
            collection.DeleteMany(filter);

            collection = database.GetCollection<BsonDocument>("RelocationOfEquipment");
            filter = Builders<BsonDocument>.Filter.Eq("from.room", id) | Builders<BsonDocument>.Filter.Eq("to.room", id);
            collection.DeleteMany(filter);

            return Ok();
        }
    }
}
