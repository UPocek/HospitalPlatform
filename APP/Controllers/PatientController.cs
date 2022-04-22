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
using Models;


namespace APP.Controllers{
[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private IMongoDatabase database;
    public PatientController()
    {
            var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
            var client = new MongoClient(settings);
            database = client.GetDatabase("USI");
    }

    // GET action

    // GET: api/Patient/doctors
    [HttpGet("doctors")]

    public IActionResult GetAllDoctors(){
            var collection = database.GetCollection<BsonDocument>("Employees");

            var filter = Builders<BsonDocument>.Filter.Eq("role", "doctor");
            var results = collection.Find(filter).ToList();
            var dotNetObjList = results.ConvertAll(BsonTypeMapper.MapToDotNetValue);
            Response.StatusCode = StatusCodes.Status200OK;
            return new JsonResult(dotNetObjList);
    }


    // GET by Id action

    // GET: api/Patient/examinations/id
    [HttpGet("examinations/{id}")]

    public async  Task<List<Examination>> GetPatientsExaminations(int id){
          var examinationCollection = database.GetCollection<Examination>("MedicalExaminations");
        return examinationCollection.Find(e => e.patinetId == id).ToList();
  }

    // POST action

    [HttpPost("examinations")]
    public async Task<IActionResult> CreateExamination(Examination examination)
    {
        var examinations = database.GetCollection<Examination>("MedicalExaminations");
        // If examination already exists abort action
        if (examinations.Find(item => item.id == examination.id).ToList().Count != 0)
        {
            return BadRequest();
        }
        
        examinations.InsertOne(examination);
        return Ok();       
    }

    // PUT action
    [HttpPut("examinations/{id}")]
        public async Task<IActionResult> UpdateExamination(Examination examination)
    {
        var examinations = database.GetCollection<Examination>("MedicalExaminations");
        // If examination doesn't exists abort action
        if (examinations.Find(item => item.id == examination.id).ToList().Count == 0)
        {
            return BadRequest();
        }
        
        var update = Builders<Examination>.Update.Set("date", examination.dateAndTimeOfExamination);
        var filter = Builders<Examination>.Filter.Eq("id", examination.id);
        examinations.UpdateOne(filter, update);
        return Ok();       
    }

    // DELETE action
    [HttpDelete("examinations/{id}")]
        public async Task<IActionResult> DeleteExamination(string id)
        {
            var examinations = database.GetCollection<Examination>("Examination");
            //izbrisi iz liste pregleda kod pacijenta i dodaj u iztoriju izmena
            examinations.DeleteOne(item => item.id == int.Parse(id));
            return Ok();
        }

}
}