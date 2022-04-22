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

    [HttpGet("examinations/doctorId/{id}")]
    public async Task<List<Examination>> GetDoctorsExaminationa(int id)
    {
        var examinationCollection = database.GetCollection<Examination>("MedicalExaminations");
        return examinationCollection.Find(e => e.doctorId == id).ToList();
    }

    [HttpGet("examinations/patientMedicalCard/{id}")]
    public async Task<MedicalCard> GetPatientMedicalCard(int id)
    {
        var examinationCollection = database.GetCollection<MedicalCard>("Patients");
        MedicalCard result = examinationCollection.Find(p => p.id == id).ToList()[0];

        // var drugCollection = database.GetCollection<BsonDocument>("Drugs");
        // List<string> patientsDrug = new List<string>();

        // for (int i = 0; i < result.medicalRecord._drugs.Count; i++){

        //     var document = new BsonDocument{
        //         {"_id", new ObjectId(result.medicalRecord._drugs[i])}
        //     };

        //     if (drugCollection.Find(document).ToList().Count != 0){
        //         patientsDrug.Add(drugCollection.Find(document).ToList()[0].ToString());
        //     }
        // }

        // result.medicalRecord.patientsDrugs = patientsDrug;

        return result;
    }

    [HttpPost("examinations")]
    public async Task<IActionResult> CreateExamination(Examination examination)
    {
        var patients = database.GetCollection<Patient>("Patients");
        var patient = patients.Find(p => p.id == examination.patinetId);
        Console.WriteLine(patient);
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
        examinations.InsertOne(examination);
        return Ok();
    }

    [HttpPut("examinations/{id}")]
    public async Task<IActionResult> UpdateExamination(int id, [FromBody] Examination examination)
    {
        var patients = database.GetCollection<Patient>("Patients");
        var patient = patients.Find(p => p.id == examination.patinetId).First();

        if (patient == null){
            return BadRequest();
        }

        var rooms = database.GetCollection<Room>("Rooms");
        var room = rooms.Find(r => r.name == examination.roomName).First();
 
        if (room == null){
            return BadRequest();
        }

        var examinationCollection = database.GetCollection<Examination>("MedicalExaminations");

        examinationCollection.ReplaceOne(e => e.id == id, examination);
        return Ok();    

    }

    [HttpDelete("examinations/{id}")]
    public async Task<IActionResult> DeleteExamination(string id)
    {
        var examinationCollection = database.GetCollection<Examination>("MedicalExaminations");
        examinationCollection.DeleteOne(e => e._id == id);
        return Ok();
    }
}

