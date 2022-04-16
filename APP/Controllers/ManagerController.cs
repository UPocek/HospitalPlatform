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

        // POST: api/Manager/renovations/1&t&t
        [HttpPost("renovations/{id}&{from}&{to}")]
        public async Task<IActionResult> CreateRenovation(string id, string from, string to)
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


            collection = database.GetCollection<BsonDocument>("Renovations");

            var document = new BsonDocument
            {
                { "room", id },
                { "startDate", from },
                { "endDate", to }
            };

            collection.InsertOne(document);

            string date = DateTime.UtcNow.ToString("yyyy-MM-dd");
            Console.WriteLine(date);

            if (date == from)
            {
                collection = database.GetCollection<BsonDocument>("Rooms");
                filter = Builders<BsonDocument>.Filter.Eq("name", id);
                var update = Builders<BsonDocument>.Update.Set("inRenovation", true);
                collection.UpdateOne(filter, update);
            }

            return Ok();
        }

        // POST: api/Manager/rooms
        [HttpPost("rooms")]
        public async Task<IActionResult> CreateRoom(string id, Data data)
        {
            var collection = database.GetCollection<BsonDocument>("Rooms");

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

            var update = Builders<BsonDocument>.Update.Set("type", data.type);
            collection.UpdateOne(filter, update);

            update = Builders<BsonDocument>.Update.Set("name", data.name);
            collection.UpdateOne(filter, update);

            collection = database.GetCollection<BsonDocument>("Renovations");
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
            collection.DeleteOne(filter);

            collection = database.GetCollection<BsonDocument>("RelocationOfEquipment");
            filter = Builders<BsonDocument>.Filter.Eq("from.room", id) | Builders<BsonDocument>.Filter.Eq("to.room", id);
            collection.DeleteOne(filter);

            return Ok();
        }
    }
}
