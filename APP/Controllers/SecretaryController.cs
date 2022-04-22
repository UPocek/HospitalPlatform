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
        public async Task<Patient> GetPatient(int id)
        {
            var collection = database.GetCollection<Patient>("Patients");
            
            return collection.Find(item => item.id == id).ToList()[0];
        }

        // POST: api/Secretary/patients
        [HttpPost("patients")]
        public async Task<IActionResult> CreatePatient(int id, Patient patient)
        {
            var collection = database.GetCollection<Patient>("Patients");

            Random rnd = new Random();
            patient.id = rnd.Next(901,10000);

            // If patient with that id already exists generate another
            do
            {
                patient.id = rnd.Next(901,10000);
            }
            while(collection.Find(item => item.id == id).ToList().Count != 0);

            collection.InsertOne(patient);

            return Ok();
        }

        // POST: api/Secretary/patients/901
        [HttpPut("patients/{id}")]
        public async Task<IActionResult> UpdatePatient(int id, Patient patient)
        {
            var patientCollection = database.GetCollection<Patient>("Patients");
            patientCollection.ReplaceOne(p => p.id == id, patient);
            return Ok();   
        }

        // DELETE: api/Secretary/patients/901

        [HttpDelete("patients/{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patientCollection = database.GetCollection<Patient>("Patients");
            patientCollection.DeleteOne(p => p.id == id);
        }

    }
}
