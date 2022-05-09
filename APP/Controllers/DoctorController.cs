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

        return examinations.Find(e => true).SortByDescending(e => e.id).FirstOrDefault();
    }

    [HttpGet("examinations/doctorId/{id}")]
    public async Task<List<Examination>> GetDoctorsExaminations(int id)
    {
        var examinations = database.GetCollection<Examination>("MedicalExaminations");

        return examinations.Find(e => e.doctorId == id).ToList();
    }

    [HttpGet("examinations/patientId/{id}")]
    public async Task<List<Examination>> GetPatientsExaminations(int id)
    {
        var examinations = database.GetCollection<Examination>("MedicalExaminations");

        return examinations.Find(e => e.patinetId == id).ToList();
    }

    [HttpGet("examinations/patientMedicalCard/{id}")]
    public async Task<MedicalCard> GetPatientMedicalCard(int id)
    {
        var patients = database.GetCollection<MedicalCard>("Patients");
        MedicalCard resultingMedicalCard = patients.Find(p => p.id == id).FirstOrDefault();

        return resultingMedicalCard;
    }

    [HttpGet("examinations/room/{name}")]
    public async Task<Room> GetExaminationRoom(string name)
    {
        var rooms = database.GetCollection<Room>("Rooms");
        Room resultingRoom = rooms.Find(r => r.name == name).FirstOrDefault();

        return resultingRoom;
    }

    [HttpGet("drugs")]
    public async Task<List<Drug>> GetDrugsForReview()
    {
        var drugs = database.GetCollection<Drug>("Drugs");
        return drugs.Find(item => item.status == "inReview").ToList();
    }

    bool IsRoomOccupied(Examination examination){
        var examinations = database.GetCollection<Examination>("MedicalExaminations");
        var possiblyOccupiedRooms = examinations.Find(item => true).ToList();

        foreach (Examination item in possiblyOccupiedRooms){
            if(item.roomName == examination.roomName){
                DateTime itemBegin = DateTime.Parse(item.dateAndTimeOfExamination);
                DateTime itemEnd = itemBegin.AddMinutes(item.durationOfExamination);
                DateTime examinationBegin = DateTime.Parse(examination.dateAndTimeOfExamination);
                DateTime examinationEnd = examinationBegin.AddMinutes(examination.durationOfExamination);
                if(examinationBegin >= itemBegin && examinationBegin <= itemEnd || examinationEnd >= itemBegin && examinationEnd <= itemEnd){
                        return true;
                }
            }  
        }
        Console.Write("Room occupied");
        return false;
    }

    bool IsRoomValid(string roomName){
        var rooms = database.GetCollection<Room>("Rooms");
        var resultingRoom = rooms.Find(r => r.name == roomName && r.inRenovation == false);
        if(resultingRoom == null){
            Console.Write("Room false");
            return false;
        }
        return true;
    }

    bool IsValidPatient(int id){
        var patients = database.GetCollection<Patient>("Patients");
        var resultingPatient = patients.Find(p => p.id == id).FirstOrDefault();
        if(resultingPatient == null){
            Console.Write("Patient false");
            return false;
        }
        return true;

    }

    bool IsRoomInRenovation(string roomName, string examinationDate){
        var renovations = database.GetCollection<Renovation>("Renovations").Find(renovation => true).ToList();

        foreach(Renovation r in renovations){
            DateTime renovationBegin = DateTime.Parse(r.startDate);
            DateTime renovationEnd = DateTime.Parse(r.endDate);
            DateTime examinationDateParsed = DateTime.Parse(examinationDate);
            if(renovationBegin <= examinationDateParsed && renovationEnd >= examinationDateParsed){
                Console.Write("Renavtion false");
                return true;
            }
        }

        return false;

    }

    [HttpPost("examinations")]
    public async Task<IActionResult> CreateExamination(Examination examination)
    {
        if ((IsValidPatient(examination.patinetId) && IsRoomValid(examination.roomName)) && (!IsRoomOccupied(examination) &&
        !IsRoomInRenovation(examination.roomName, examination.dateAndTimeOfExamination)))
        {
            var examinations = database.GetCollection<Examination>("MedicalExaminations");
            var id = examinations.Find(e => true).SortByDescending(e => e.id).FirstOrDefault().id;
            examination.id = id + 1;
            examinations.InsertOne(examination);

            return Ok(); 
        }
        else{
            return BadRequest();       
        }
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
    public async Task<IActionResult> UpdateExamination(int id, [FromBody] Examination examination)
    {
        var patients = database.GetCollection<Patient>("Patients");
        var resultingPatient = patients.Find(p => p.id == examination.patinetId).FirstOrDefault();
        var rooms = database.GetCollection<Room>("Rooms");
        var resultingRoom = rooms.Find(r => r.name == examination.roomName).FirstOrDefault();
 

        if (resultingPatient == null | resultingRoom == null){
            return BadRequest();
        }
        else{
            var examinations = database.GetCollection<Examination>("MedicalExaminations");
            examinations.FindOneAndReplace(e => e.id == id, examination);
            
            return Ok(); 
        }   
    }

    [HttpPut("examinations/room/{name}")]
    public async Task<IActionResult> UpdateExaminationRoom(string name, [FromBody] Room room)
    {
        var rooms = database.GetCollection<Room>("Rooms");
        
        rooms.FindOneAndReplace(r => r.name == name, room);
        return Ok();    
    }

    [HttpPut("examinations/medicalrecord/{id}")]
    public async Task<IActionResult> UpdateMedicalCard(int id, MedicalRecord medicalRecord )
    {
        var patients = database.GetCollection<Patient>("Patients");
        var updatePatients = Builders<Patient>.Update.Set("medicalRecord", medicalRecord);
        patients.UpdateOne(p => p.id == id, updatePatients);
        return Ok();    
    }

    [HttpDelete("examinations/{id}")]
    public async Task<IActionResult> DeleteExamination(int id)
    {
        var examinations = database.GetCollection<Examination>("MedicalExaminations");
        examinations.DeleteOne(e => e.id == id);
        return Ok();
    }
}

