#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
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
        public async Task<List<Patient>> GetPatients()
        {
            var collection = database.GetCollection<Patient>("Patients");

            return collection.Find(item => true).ToList();
        }

        // GET by Id: api/Secretary/patients/901
        [HttpGet("patients/{id}")]
        public async Task<Patient> GetPatient(string id)
        {
            var collection = database.GetCollection<Patient>("Patients");
            
            return collection.Find(item => item.id == id).ToList()[0];
        }

        // POST: api/Secretary/patients
        [HttpPost("patients")]
        public async Task<IActionResult> CreatePatient(string id, Patient patient)
        {
            var collection = database.GetCollection<Patient>("Patients");

            Random rnd = new Random();
            patient.id = rnd.Next(901,10000).ToString();

            // If patient with that id already exists generate another
            do
            {
                patient.id = rnd.Next(901,10000).ToString();
            }
            while(collection.Find(item => item.id == id).ToList().Count != 0);

            collection.InsertOne(patient);

            return Ok();
        }

        // PUT action

        // DELETE action

    }
}
