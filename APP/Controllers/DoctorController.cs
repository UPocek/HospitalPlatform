#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Models;
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
        IMongoCollection<Examination> examinationCollection = database.GetCollection<Examination>("MedicalExaminations");
        return examinationCollection.Find(e => true).ToList();
    }

    [HttpGet("examinations/doctor_id/{id}")]
    public async Task<List<Examination>> GetDoctorsExaminationa(int id)
    {
        IMongoCollection<Examination> examinationCollection = database.GetCollection<Examination>("MedicalExaminations");
        return examinationCollection.Find(e => e.doctorId == id).ToList();
    }

    [HttpGet("examinations/patient_medical_card/{id}")]
    public async Task<MedicalCard> GetPatientMedicalCard(int id)
    {
        IMongoCollection<MedicalCard> examinationCollection = database.GetCollection<MedicalCard>("Patients");
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
    public async Task<IActionResult> CreateExamination([FromBody] Examination examination)
    {
        IMongoCollection<Examination> examinationCollection = database.GetCollection<Examination>("MedicalExaminations");
        examinationCollection.InsertOne(examination);
        return Ok();       
    }

    [HttpPut("examinations/{id}")]
    public async Task<IActionResult> UpdateExamination(string id, [FromBody] Examination examination)
    {
        IMongoCollection<Examination> examinationCollection = database.GetCollection<Examination>("MedicalExaminations");
        examinationCollection.ReplaceOne(e => e.id == id, examination);
        return Ok();    
    }

    [HttpDelete("examinations/{id}")]
    public async Task<IActionResult> DeleteExamination(string id)
    {
        IMongoCollection<Examination> examinationCollection = database.GetCollection<Examination>("MedicalExaminations");
        examinationCollection.DeleteOne(e => e.id == id);
        return Ok(); 
    }
    
}
