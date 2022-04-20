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

    public IActionResult GetDoctors(){
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

    public IActionResult GetExaminations(string id){
            var collection = database.GetCollection<BsonDocument>("MedicalExaminations");

            var filter = Builders<BsonDocument>.Filter.Eq("patient", Int64.Parse(id));
            var results = collection.Find(filter).ToList();
            var dotNetObjList = results.ConvertAll(BsonTypeMapper.MapToDotNetValue);
            Response.StatusCode = StatusCodes.Status200OK;
            return new JsonResult(dotNetObjList);
    }

    // POST action

    // PUT action

    // DELETE action
}
}