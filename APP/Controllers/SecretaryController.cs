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

        // POST: api/Secretary/patients
        [HttpPost("patients")]
        public async Task<IActionResult> CreatePatient(string id, Patient patient)
        {
            var collection = database.GetCollection<BsonDocument>("Patients");

            Random rnd = new Random();
            patient.id = rnd.Next(901,10000).ToString();

            // If patient with that id already exists generate another
            do
            {
                patient.id = rnd.Next(901,10000).ToString();
            }
            while(collection.Find(Builders<BsonDocument>.Filter.Eq("id", patient.id)).ToList().Count != 0);

            var document = new BsonDocument
            {
                {"firstName", patient.firstName },
                {"lastName", patient.lastName},
                {"role", "patient"},
                {"email", patient.email},
                {"password", patient.password},
                {"active", patient.active},
                {"id", patient.id},
                {"medicalRecord", new BsonDocument{
                        {"height", patient.medicalRecord.height},
                        {"weight", patient.medicalRecord.weight},
                        {"diseases", new BsonArray()},
                        {"alergies", new BsonArray()},
                        {"drugs", new BsonArray()},
                        {"examinations", new BsonArray()},
                        {"medicalInstructions", new BsonArray()}

                    }
                },
            };

            collection.InsertOne(document);

            return Ok();
        }

        // PUT action

        // DELETE action

    }
}
