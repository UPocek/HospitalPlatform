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
    public class SecretaryController : ControllerBase
    {

        private IMongoDatabase database;
        public SecretaryController()
        {
            var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
            var client = new MongoClient(settings);
            database = client.GetDatabase("USI");
        }

        // GET: api/Secretary/patients
        [HttpGet("patients")]
        public IActionResult GetPatients()
        {
            var collection = database.GetCollection<BsonDocument>("Patients");
            var filter = Builders<BsonDocument>.Filter.Empty;
            var results = collection.Find(filter).ToList();
            var dotNetObjList = results.ConvertAll(BsonTypeMapper.MapToDotNetValue);
            Response.StatusCode = StatusCodes.Status200OK;
            return new JsonResult(dotNetObjList);
        }

        // GET by Id: api/Secretary/patients/901
        [HttpGet("patients/{id}")]
        public IActionResult GetPatient(string id)
        {
            var collection = database.GetCollection<BsonDocument>("Patients");
            var filter = Builders<BsonDocument>.Filter.Eq("id", id);
            var result = collection.Find(filter).First();
            var dotNetObj = BsonTypeMapper.MapToDotNetValue(result);
            Response.StatusCode = StatusCodes.Status200OK;
            return new JsonResult(dotNetObj);
        }

        // GET by Id action

        // POST action

        // PUT action

        // DELETE action

    }
}
