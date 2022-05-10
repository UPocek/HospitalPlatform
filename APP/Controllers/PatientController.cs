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
          var examinations = database.GetCollection<Examination>("MedicalExaminations");
          List<Examination> patientsExaminations = examinations.Find(e => e.PatinetId == id).ToList();
        return patientsExaminations;
  }

    // GET: api/Patient/examination/id
    [HttpGet("examination/{id}")]

    public async  Task<Examination> GetExamination(int id){
          var examinations = database.GetCollection<Examination>("MedicalExaminations");
          Examination patientsExaminations = examinations.Find(e => e.Id == id).FirstOrDefault();
        return patientsExaminations;
  }


    // POST action

    [HttpPost("examinations")]
    public async Task<IActionResult> CreateExamination(Examination examination)
    {
        var patients = database.GetCollection<Patient>("Patients");
        Patient patient = patients.Find(p => p.Id == examination.PatinetId).FirstOrDefault();

        var isTroll = trollCheck(patient, "created", 8);
        if(!isTroll){
            return BadRequest();
            //blokiraj pacijenta
        }
        var examinations = database.GetCollection<Examination>("MedicalExaminations");

        var doctorsExaminations = examinations.Find(item => item.DoctorId == examination.DoctorId).ToList();
        foreach (var item in doctorsExaminations){
                DateTime itemBegin = DateTime.Parse(item.DateAndTimeOfExamination);
                DateTime itemEnd = itemBegin.AddMinutes(item.DurationOfExamination);
                DateTime examinationBegin = DateTime.Parse(examination.DateAndTimeOfExamination);
                DateTime examinationEnd = examinationBegin.AddMinutes(examination.DurationOfExamination);
                if(examinationBegin >= itemBegin && examinationBegin <= itemEnd || examinationEnd >= itemBegin && examinationEnd <= itemEnd){
                        return BadRequest();
                }
        }        

        var rooms = database.GetCollection<Room>("Rooms");
        var validRooms = rooms.Find(room => room.InRenovation == false && room.Type == "examination room").ToList();
        
        foreach (var room  in validRooms)
        {
            var examinationsInRoom = examinations.Find(item => item.RoomName == room.Name && item.DateAndTimeOfExamination != examination.DateAndTimeOfExamination).ToList();
            if(examinationsInRoom != null){
                examination.RoomName = room.Name;
                break;
            }
             
        }


        var id = examinations.Find(e => true).SortByDescending(e => e.Id).FirstOrDefault().Id;
        examination.Id = id + 1;
        examinations.InsertOne(examination);

        ExaminationHistoryEntry newEntry = new ExaminationHistoryEntry();
        newEntry.Date = DateTime.Today.ToString();
        newEntry.Type = "created";
        var update = Builders<Patient>.Update.Push("examinationHistory", newEntry);
        patients.UpdateOne(p => p.Id == patient.Id, update);

        return Ok();       
    }

    [HttpPost("examinationRequests")]
    public async Task<IActionResult> createRequest(ExaminationRequest request)
    {
        var requests = database.GetCollection<ExaminationRequest>("ExaminationRequests");
        requests.InsertOne(request);
        return Ok(); 
    }


    // PUT action
    [HttpPut("examinations/{id}")]
        public async Task<IActionResult> UpdateExamination(string id,[FromBody] Examination examination)
    {
        var patients = database.GetCollection<Patient>("Patients");
        Patient patient = patients.Find(p => p.Id == examination.PatinetId).FirstOrDefault();

        var isTroll = trollCheck(patient, "deleted/updated", 5);
        if(!isTroll){
            return BadRequest();
            //blokiraj pacijenta
        }
        var examinations = database.GetCollection<Examination>("MedicalExaminations");
        var oldExaminationData = examinations.Find(item => item.Id == int.Parse(id)).FirstOrDefault();
        examination._Id = oldExaminationData._Id;
        examination.Id = oldExaminationData.Id;

        var doctorsExaminations = examinations.Find(item => item.DoctorId == examination.DoctorId).ToList();
        foreach (var item in doctorsExaminations){
                DateTime itemBegin = DateTime.Parse(item.DateAndTimeOfExamination);
                DateTime itemEnd = itemBegin.AddMinutes(item.DurationOfExamination);
                DateTime examinationBegin = DateTime.Parse(examination.DateAndTimeOfExamination);
                DateTime examinationEnd = examinationBegin.AddMinutes(examination.DurationOfExamination);
                if(examinationBegin >= itemBegin && examinationBegin <= itemEnd || examinationEnd >= itemBegin && examinationEnd <= itemEnd){
                        return BadRequest();
                }
        }        

        var rooms = database.GetCollection<Room>("Rooms");
        var validRooms = rooms.Find(room => room.InRenovation == false && room.Type == "examination room").ToList();

        foreach (var room  in validRooms)
        {
            var examinationsInRoom = examinations.Find(item => item.RoomName == room.Name && item.DateAndTimeOfExamination != examination.DateAndTimeOfExamination).ToList();
            if(examinationsInRoom != null){
                examination.RoomName = room.Name;
                break;
            }
             
        }

        DateTime dt = DateTime.Today;
        DateTime dtOfExamination = DateTime.Parse(oldExaminationData.DateAndTimeOfExamination);
        
        if(dt.AddDays(2) >= dtOfExamination){
            ExaminationRequest request = new ExaminationRequest();
            request.Examination = examination;
            request.Status = 1;
            await createRequest(request);
            return Ok();
        } 
        examinations.FindOneAndReplace(e => e.Id == int.Parse(id), examination);
        
        ExaminationHistoryEntry newEntry = new ExaminationHistoryEntry();
        newEntry.Date = DateTime.Today.ToString();
        newEntry.Type = "updated";
        var update = Builders<Patient>.Update.Push("examinationHistory", newEntry);
        patients.UpdateOne(p => p.Id == patient.Id, update);
    
        return Ok();
     
    }

    // DELETE action
    [HttpDelete("examinations/{id}")]
        public async Task<IActionResult> DeleteExamination(string id)
        {
            var examinations = database.GetCollection<Examination>("MedicalExaminations");
            Examination examination = examinations.Find(item => item.Id == int.Parse(id)).FirstOrDefault();
            var patients = database.GetCollection<Patient>("Patients");
            Patient patient = patients.Find(p => p.Id == examination.PatinetId).FirstOrDefault();

            var isTroll = trollCheck(patient, "deleted/updated", 5);
            if(!isTroll){
                return BadRequest();
            }

            
            DateTime dt = DateTime.Today;
            DateTime dtOfExamination = DateTime.Parse(examination.DateAndTimeOfExamination);

        if(dt.AddDays(2) >= dtOfExamination){
            ExaminationRequest request = new ExaminationRequest();
            request.Examination = examination;
            request.Status = 0;
            await createRequest(request);
            return Ok();
        }
                
            examinations.DeleteOne(item => item.Id == int.Parse(id));


            ExaminationHistoryEntry newEntry = new ExaminationHistoryEntry();
            newEntry.Date = DateTime.Today.ToString();
            newEntry.Type = "deleted";
            var update = Builders<Patient>.Update.Push("examinationHistory", newEntry);
            patients.UpdateOne(p => p.Id == patient.Id, update);
    

            return Ok();

           
        }


public Boolean trollCheck(Patient patient,String type, int n){
    var patients = database.GetCollection<Patient>("Patients");
    DateTime checkDate = DateTime.Today.AddDays(-30);
    int counter = 0;
    foreach(var entry in patient.ExaminationHistory){
        DateTime entryDate = DateTime.Parse(entry.Date);
        if (entryDate > checkDate && type.Contains(entry.Type)){
            counter++;
        }
        if (entryDate < checkDate){
            var update = Builders<Patient>.Update.PopFirst("examinationHistory");
            patients.UpdateOne(p => p.Id == patient.Id, update);
        }
    }

    if (counter>n){
        var block = Builders<Patient>.Update.Set("active", "2");
        patients.UpdateOne(p => p.Id == patient.Id, block);
        return false;
    }
    return true;
}

}
}