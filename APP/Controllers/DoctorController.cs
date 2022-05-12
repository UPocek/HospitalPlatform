#nullable disable
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

[ApiController]
[Route("api/[controller]")]
public class DoctorController : ControllerBase
{
    private IMongoDatabase database;
    public DoctorController()
    {
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        database = client.GetDatabase("USI");
    }

    [HttpGet("examinations")]
    public async Task<List<Examination>> GetAllExaminations()
    {
        var examinations = database.GetCollection<Examination>("MedicalExaminations");

        return examinations.Find(e => true).ToList();
    }

    [HttpGet("examinations/nextIndex")]
    public async Task<Examination> GetNextExaminationsIndex()
    {
        var examinations = database.GetCollection<Examination>("MedicalExaminations");

        return examinations.Find(e => true).SortByDescending(e => e.Id).FirstOrDefault();
    }

    [HttpGet("examinations/doctorId/{id}")]
    public async Task<List<Examination>> GetDoctorsExaminations(int id)
    {
        var examinations = database.GetCollection<Examination>("MedicalExaminations");

        return examinations.Find(e => e.DoctorId == id).ToList();
    }

    [HttpGet("examinations/patientId/{id}")]
    public async Task<List<Examination>> GetPatientsExaminations(int id)
    {
        var examinations = database.GetCollection<Examination>("MedicalExaminations");

        return examinations.Find(e => e.PatinetId == id).ToList();
    }

    [HttpGet("examinations/patientMedicalCard/{id}")]
    public async Task<MedicalCard> GetPatientMedicalCard(int id)
    {
        var patients = database.GetCollection<MedicalCard>("Patients");
        MedicalCard resultingMedicalCard = patients.Find(p => p.Id == id).FirstOrDefault();

        return resultingMedicalCard;
    }

    [HttpGet("examinations/room/{name}")]
    public async Task<Room> GetExaminationRoom(string name)
    {
        var rooms = database.GetCollection<Room>("Rooms");
        Room resultingRoom = rooms.Find(r => r.Name == name).FirstOrDefault();

        return resultingRoom;
    }

    [HttpGet("drugs")]
    public async Task<List<Drug>> GetDrugsForReview()
    {
        var drugs = database.GetCollection<Drug>("Drugs");

        return drugs.Find(item => item.Status == "inReview").ToList();
    }

    public bool IsRoomOccupied(Examination examination){
        var examinations = database.GetCollection<Examination>("MedicalExaminations");
        var possiblyOccupiedRooms = examinations.Find(item => true).ToList();

        foreach (Examination item in possiblyOccupiedRooms){
            if(item.RoomName == examination.RoomName){
                DateTime itemBegin = DateTime.Parse(item.DateAndTimeOfExamination);
                DateTime itemEnd = itemBegin.AddMinutes(item.DurationOfExamination);
                DateTime examinationBegin = DateTime.Parse(examination.DateAndTimeOfExamination);
                DateTime examinationEnd = examinationBegin.AddMinutes(examination.DurationOfExamination);
                if(examinationBegin >= itemBegin && examinationBegin <= itemEnd || examinationEnd >= itemBegin && examinationEnd <= itemEnd){
                        return true;
                }
            }  
        }
        return false;
    }

    public bool IsRoomValid(string roomName){
        var rooms = database.GetCollection<Room>("Rooms");
        var resultingRoom = rooms.Find(r => r.Name == roomName && r.InRenovation == false);
        if(resultingRoom == null){
            return false;
        }
        return true;
    }

    public bool IsValidPatient(int id){
        var patients = database.GetCollection<Patient>("Patients");
        var resultingPatient = patients.Find(p => p.Id == id).FirstOrDefault();
        if(resultingPatient == null){
            return false;
        }
        return true;
    }

    public bool IsRoomInRenovation(string roomName, string examinationDate){
        var renovations = database.GetCollection<Renovation>("Renovations").Find(renovation => true).ToList();

        foreach(Renovation r in renovations){
            DateTime renovationBegin = DateTime.Parse(r.StartDate);
            DateTime renovationEnd = DateTime.Parse(r.EndDate);
            DateTime examinationDateParsed = DateTime.Parse(examinationDate);
            if(renovationBegin <= examinationDateParsed && renovationEnd >= examinationDateParsed){
                return true;
            }
        }

        return false;

    }

    [HttpPost("examinations")]
    public async Task<IActionResult> CreateExamination(Examination examination)
    {
        if ((IsValidPatient(examination.PatinetId) && IsRoomValid(examination.RoomName)) && (!IsRoomOccupied(examination) &&
        !IsRoomInRenovation(examination.RoomName, examination.DateAndTimeOfExamination)))
        {
            var examinations = database.GetCollection<Examination>("MedicalExaminations");
            var id = examinations.Find(e => true).SortByDescending(e => e.Id).FirstOrDefault().Id;
            examination.Id = id + 1;
            examinations.InsertOne(examination);

            return Ok(); 
        }
        return BadRequest();
    }

    [HttpPut("drugs/{id}")]
    public async Task<IActionResult> UpdateDrugMessage(string id, Dictionary<string, string> data)
    {
        var drugs = database.GetCollection<Drug>("Drugs");
        
        var filter = Builders<Drug>.Filter.Eq("name", id);
        var update = Builders<Drug>.Update.Set("comment", data["message"]);
        drugs.UpdateOne(filter, update);

        return Ok();
    }

    [HttpPut("drugs/approve/{id}")]
    public async Task<IActionResult> ApproveDrug(string id)
    {
        var drugs = database.GetCollection<Drug>("Drugs");
        
        var filter = Builders<Drug>.Filter.Eq("name", id);
        var update = Builders<Drug>.Update.Set("status", "inUse");
        drugs.UpdateOne(filter, update);

        return Ok();
    }

    [HttpPut("examinations/{id}")]
    public async Task<IActionResult> UpdateExamination(int id, Examination examination)
    {
        if ((IsValidPatient(examination.PatinetId) && IsRoomValid(examination.RoomName)) && (!IsRoomOccupied(examination) &&
        !IsRoomInRenovation(examination.RoomName, examination.DateAndTimeOfExamination)))
        {
            var examinations = database.GetCollection<Examination>("MedicalExaminations");
            examinations.FindOneAndReplace(e => e.Id == id, examination);
            
            return Ok(); 
        } 
        return BadRequest();
    }

    [HttpPut("examinations/room/{name}")]
    public async Task<IActionResult> UpdateExaminationRoom(string name, Room room)
    {
        var rooms = database.GetCollection<Room>("Rooms");
        
        rooms.FindOneAndReplace(r => r.Name == name, room);
        return Ok();    
    }

    [HttpPut("examinations/medicalrecord/{id}")]
    public async Task<IActionResult> UpdateMedicalCard(int id, MedicalRecord medicalRecord )
    {
        var patients = database.GetCollection<Patient>("Patients");
        var updatePatients = Builders<Patient>.Update.Set("medicalRecord", medicalRecord);
        patients.UpdateOne(p => p.Id == id, updatePatients);
        return Ok();    
    }

    [HttpDelete("examinations/{id}")]
    public async Task<IActionResult> DeleteExamination(int id)
    {
        var examinations = database.GetCollection<Examination>("MedicalExaminations");
        examinations.DeleteOne(e => e.Id == id);
        
        return Ok();
    }
}

