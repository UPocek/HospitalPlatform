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
        var patients = database.GetCollection<Patient>("Patients");
        var patient = patients.Find(p => p.id == examination.patinetId).First();

        var doctors = database.GetCollection<Employee>("Employees");
        var doctor = doctors.Find(d => d.id == examination.doctorId);
        if(doctor == null){
                return BadRequest();
        }

        var examinations = database.GetCollection<Examination>("MedicalExaminations");

        var doctorsExaminations = examinations.Find(item => item.doctorId == examination.doctorId).ToList();
        foreach (var item in doctorsExaminations){
                if(item.dateAndTimeOfExamination == examination.dateAndTimeOfExamination){
                        return BadRequest();
                }
        }       

        var rooms = database.GetCollection<Room>("Rooms");
        var validRooms = rooms.Find(room => room.inRenovation == false && room.type == "examination room").ToList();
        
        foreach (var room  in validRooms)
        {
            var examinationsInRoom = examinations.Find(item => item.roomName == room.name && item.dateAndTimeOfExamination != examination.dateAndTimeOfExamination).ToList();
            if(examinationsInRoom != null){
                examination.roomName = room.name;
                break;
            }
             
        }


        var id = examinations.Find(e => true).SortByDescending(e => e.id).First().id;
        examination.id = id + 1;
        examinations.InsertOne(examination);

        ExaminationHistoryEntry newEntry = new ExaminationHistoryEntry();
        newEntry.date = DateTime.Today.ToString();
        newEntry.type = "created";
        patient.examinationHistory.Add(newEntry);

        return Ok();       
    }

    // PUT action
    [HttpPut("examinations/{id}")]
        public async Task<IActionResult> UpdateExamination(int id, Examination examination)
    {
       
        var doctors = database.GetCollection<Employee>("Employees");
        var doctor = doctors.Find(d => d.id == examination.doctorId);
        if(doctor == null){
                return BadRequest();
        }

        var examinations = database.GetCollection<Examination>("MedicalExaminations");
        Examination oldExaminationData = examinations.Find(item => item.id == examination.id).First();

        var doctorsExaminations = examinations.Find(item => item.doctorId == examination.doctorId).ToList();
        foreach (var item in doctorsExaminations){
                if(item.dateAndTimeOfExamination == examination.dateAndTimeOfExamination){
                        return BadRequest();

                }
        }    

        DateTime dt = DateTime.Today;
        DateTime dtOfExamination = DateTime.Parse(oldExaminationData.dateAndTimeOfExamination);
        if(dt.Day+1<dtOfExamination.Day){
        var filter = Builders<Examination>.Filter.Eq("id", id);
        var update = Builders<Examination>.Update.Set("doctor", examination.doctorId);
        examinations.UpdateOne(filter, update);
        update = Builders<Examination>.Update.Set("date", examination.dateAndTimeOfExamination);
        examinations.UpdateOne(filter, update);
        return Ok();   
        }

        return BadRequest();

     
    }

    // DELETE action
    [HttpDelete("examinations/{id}")]
        public async Task<IActionResult> DeleteExamination(string id)
        {
            var examinations = database.GetCollection<Examination>("MedicalExaminations");
            Examination examination = examinations.Find(item => item.id == int.Parse(id)).First();
            var patients = database.GetCollection<Patient>("Patients");
            DateTime dt = DateTime.Today;
            DateTime dtOfExamination = DateTime.Parse(examination.dateAndTimeOfExamination);
            if(dt.Day+1<dtOfExamination.Day){
                examinations.DeleteOne(item => item.id == int.Parse(id));
                return Ok();
            }

            //inace posalji zahtev
            return BadRequest();
            //izbrisi iz liste pregleda kod pacijenta i dodaj u istoriju izmena
           
        }

}
}