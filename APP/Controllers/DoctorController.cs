#nullable disable
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Models;

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

    [HttpGet("examinations/{id}")]
    public async Task<Examination> GetExamination(string id)
    {
        IMongoCollection<Examination> examinationCollection = database.GetCollection<Examination>("MedicalExaminations");
        return examinationCollection.Find(e => e.id == id).FirstOrDefault();
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
