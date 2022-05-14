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

    // POST: api/Patient/examinationFilter
    [HttpPost("examinationFilter")]

    public async  Task<List<Examination>> GetAvailableExamination([FromBody] ExaminationFilter filter){
        var dueDate = DateTime.Parse(filter.dueDate);
        var timeFrom = DateTime.Parse(filter.timeFrom);
        var timeTo = DateTime.Parse(filter.timeTo);
        var intervalFrom = DateTime.Today.AddDays(1).AddHours(timeFrom.Hour).AddMinutes(timeFrom.Minute);
        var intervalEnd = DateTime.Today.AddDays(1).AddHours(timeTo.Hour).AddMinutes(timeTo.Minute);
        var result = new List<Examination>();
        
        var examinations = database.GetCollection<Examination>("MedicalExaminations");
        var doctorsExaminations = examinations.Find(e => e.doctorId == filter.doctorId).ToList();
        var patientsExaminations = examinations.Find(e => e.patinetId == filter.patientId).ToList();
        var examinationTime = intervalFrom;

        int i = 0;
        var okay = true;
        while(examinationTime < dueDate){
            
            Examination examination = new Examination();
            examination.doctorId = filter.doctorId;
            examination.dateAndTimeOfExamination = examinationTime.ToString();
            examination.typeOfExamination = "visit";

            //check if doctor is free        
            foreach(var item in doctorsExaminations){
                DateTime itemBegin = DateTime.Parse(item.dateAndTimeOfExamination);
                DateTime itemEnd = itemBegin.AddMinutes(item.durationOfExamination);

                if(examinationTime >= itemBegin && examinationTime <= itemEnd || examinationTime.AddMinutes(15) >= itemBegin && examinationTime.AddMinutes(15) <= itemEnd){
                    okay = false;
                    break; 
                }

            }
            //check if patient is free        
            foreach(var item in patientsExaminations){
                DateTime itemBegin = DateTime.Parse(item.dateAndTimeOfExamination);
                DateTime itemEnd = itemBegin.AddMinutes(item.durationOfExamination);

                if(examinationTime >= itemBegin && examinationTime <= itemEnd || examinationTime.AddMinutes(15) >= itemBegin && examinationTime.AddMinutes(15) <= itemEnd){
                    okay = false; 
                    break;
                }
            }
            if(okay){
                result.Add(examination);
                i++;
            }    
            examinationTime = examinationTime.AddMinutes(15);
            if(examinationTime.AddMinutes(15) > intervalEnd){
                intervalFrom = intervalFrom.AddDays(1);
                intervalEnd = intervalEnd.AddDays(1);
                examinationTime = intervalFrom;    
            }  
            okay = true;
        }

        //ako nema termina za odabrnog doktora u odabrano vreme
        if (result.Count == 0){
            if(filter.priority == "doctor"){
                
            }else if(filter.priority == "time"){ 

            }
        }

        return result;
  }


    // POST action

    [HttpPost("examinations")]
    public async Task<IActionResult> CreateExamination(Examination examination)
    {        
        var examinations = database.GetCollection<Examination>("MedicalExaminations");
        DateTime examinationBegin = DateTime.Parse(examination.dateAndTimeOfExamination);
        DateTime examinationEnd = examinationBegin.AddMinutes(examination.durationOfExamination);

        var patients = database.GetCollection<Patient>("Patients");
        Patient patient = patients.Find(p => p.Id == examination.PatinetId).FirstOrDefault();

        var patientsExaminations = examinations.Find(item => item.patinetId == examination.patinetId).ToList();
        foreach (var item in patientsExaminations){
                DateTime itemBegin = DateTime.Parse(item.dateAndTimeOfExamination);
                DateTime itemEnd = itemBegin.AddMinutes(item.durationOfExamination);

                if(examinationBegin >= itemBegin && examinationBegin <= itemEnd || examinationEnd >= itemBegin && examinationEnd <= itemEnd){
                        return BadRequest();
                }
        }   

        var isTroll = trollCheck(patient, "created", 8);
        if(!isTroll){
            return BadRequest();
        }                

        
        var doctorsExaminations = examinations.Find(item => item.doctorId == examination.doctorId).ToList();
        foreach (var item in doctorsExaminations){
                DateTime itemBegin = DateTime.Parse(item.dateAndTimeOfExamination);
                DateTime itemEnd = itemBegin.AddMinutes(item.durationOfExamination);

                if(examinationBegin >= itemBegin && examinationBegin <= itemEnd || examinationEnd >= itemBegin && examinationEnd <= itemEnd){
                        return BadRequest();
                }
        }       

        if(examination.doctorId % 2 != 0){
            examination.roomName = "Examination room 1"; 
        }else{
            examination.roomName = "Examination room 2"; 
        }
        

        var rooms = database.GetCollection<Room>("Rooms");
        var validRooms = rooms.Find(room => room.inRenovation == false && room.type == "examination room").ToList();
        if(examination.doctorId % 2 != 0){
            examination.roomName = validRooms[0].name; 
        }else{
            examination.roomName = validRooms[validRooms.Count-1].name; 
        }
        
        // foreach (var room  in validRooms)
        // {
        //     var examinationsInRoom = examinations.Find(item => item.roomName == room.name && item.dateAndTimeOfExamination != examination.dateAndTimeOfExamination).ToList();
            
        //     if(examinationsInRoom != null){
        //         examination.roomName = room.name;
        //         break;
        //     }
             
        // }


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
        examination._id = oldExaminationData._id;
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