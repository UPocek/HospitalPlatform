#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Bson;

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
        var examinationCollection = database.GetCollection<Examination>("MedicalExaminations");
        return examinationCollection.Find(e => true).ToList();
    }

    [HttpGet("examinations/nextIndex")]
    public async Task<Examination> GetNextExaminationsIndex()
    {
        var examinationCollection = database.GetCollection<Examination>("MedicalExaminations");
        return examinationCollection.Find(e => true).SortByDescending(e => e.id).FirstOrDefault();
    }

    [HttpGet("examinations/doctorId/{id}")]
    public async Task<List<Examination>> GetDoctorsExaminationa(int id)
    {
        var examinationCollection = database.GetCollection<Examination>("MedicalExaminations");
        return examinationCollection.Find(e => e.doctorId == id).ToList();
    }

    [HttpGet("examinations/patientId/{id}")]
    public async Task<List<Examination>> GetPatientsExaminationa(int id)
    {
        var examinationCollection = database.GetCollection<Examination>("MedicalExaminations");
        return examinationCollection.Find(e => e.patinetId == id).ToList();
    }

    [HttpGet("examinations/patientMedicalCard/{id}")]
    public async Task<MedicalCard> GetPatientMedicalCard(int id)
    {
        var patientsCards = database.GetCollection<MedicalCard>("Patients");
        MedicalCard result = patientsCards.Find(p => p.id == id).FirstOrDefault();

        return result;
    }

    [HttpGet("examinations/room/{name}")]
    public async Task<Room> GetExaminationRoom(string name)
    {
        var rooms = database.GetCollection<Room>("Rooms");
        Room result = rooms.Find(r => r.name == name).FirstOrDefault();

        return result;
    }

    [HttpPost("examinations")]
    public async Task<IActionResult> CreateExamination(Examination examination)
    {
        var patients = database.GetCollection<Patient>("Patients");
        var patient = patients.Find(p => p.id == examination.patinetId).FirstOrDefault();
        
        if (patient == null)
        {
            return BadRequest();
        }

        var rooms = database.GetCollection<Room>("Rooms");
        var room = rooms.Find(r => r.name == examination.roomName);

        if (room == null)
        {
            return BadRequest();
        }

        var examinations = database.GetCollection<Examination>("MedicalExaminations");
        var id = examinations.Find(e => true).SortByDescending(e => e.id).FirstOrDefault().id;
        examination.id = id + 1;
        examinations.InsertOne(examination);
        return Ok();
    }

    [HttpPut("examinations/{id}")]
    public async Task<IActionResult> UpdateExamination(int id, [FromBody] Examination examination)
    {
        var patients = database.GetCollection<Patient>("Patients");
        var patient = patients.Find(p => p.id == examination.patinetId).FirstOrDefault();

        if (patient == null){
            return BadRequest();
        }

        var rooms = database.GetCollection<Room>("Rooms");
        var room = rooms.Find(r => r.name == examination.roomName).FirstOrDefault();
 
        if (room == null){
            return BadRequest();
        }

        var examinationCollection = database.GetCollection<Examination>("MedicalExaminations");

        Console.WriteLine(id);
        Console.WriteLine(examination.id);

        examinationCollection.FindOneAndReplace(e => e.id == id, examination);
        return Ok();    

    }

    [HttpPut("examinations/room/{name}")]
    public async Task<IActionResult> UpdateExaminationRoom(string name, [FromBody] Room room)
    {
        var rooms = database.GetCollection<Room>("Rooms");
        
        rooms.FindOneAndReplace(e => e.name == name, room);
        return Ok();    

    }

    [HttpPut("examinations/medicalrecord/{id}")]
    public async Task<IActionResult> UpdateMedicalCard(int id, MedicalRecord medicalRecord )
    {
        var patients = database.GetCollection<Patient>("Patients");
        var updatePatients = Builders<Patient>.Update.Set("medicalRecord", medicalRecord);
        patients.UpdateOne(item => item.id == id, updatePatients);
        return Ok();    

    }

    [HttpDelete("examinations/{id}")]
    public async Task<IActionResult> DeleteExamination(int id)
    {
        var examinationCollection = database.GetCollection<Examination>("MedicalExaminations");
        examinationCollection.DeleteOne(e => e.id == id);
        return Ok();
    }
}

