#nullable disable
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net.Mail;

[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private IMongoDatabase _database;

    private IPatientService _patientService;
    public PatientController()
    {
        _patientService = new PatientService();
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        _database = client.GetDatabase("USI");
    }
    //From here SOLID

    [HttpGet("")]
    public async Task<List<Patient>> GetAllPatients()
    {
        return await _patientService.GetAllPatients();
    }

    [HttpGet("{id}")]
    public async Task<Patient> GetPatientById(int id)
    {
        return await _patientService.GetPatientById(id);
    }

    [HttpGet("unblocked")]
    public async Task<List<Patient>> GetUnblockedPatients()
    {
        return await _patientService.GetUnblockedPatients();
    }

    [HttpGet("blocked")]
    public async Task<List<Patient>> GetBlockedPatients()
    {
        return await _patientService.GetBlockedPatients();
    }

    [HttpGet("unblocked/{id}")]
    public async Task<Patient> GetUnblockedPatient(int id)
    {
        return await _patientService.GetUnblockedPatient(id);
    }

    [HttpGet("activity/{id}")]
    public async Task<String> GetPatientActivity(int id)
    {
        return await _patientService.GetPatientActivity(id);
    }
    [HttpPost("create")]
    public async Task<IActionResult> CreatePatient(Patient patient)
    {
        if (!await _patientService.isNewPatientValid(patient))
        {
            return BadRequest();
        }
        await _patientService.CreatePatient(patient);
        return Ok();
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePatient(int id, Patient patient)
    {
        await _patientService.UpdatePatient(id, patient);
        return Ok();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePatient(int id)
    {
        await _patientService.DeletePatient(id);
        return Ok();
    }

    [HttpPut("activity/{id}/{activityValue}")]
    public async Task<IActionResult> UpdatePatientActivity(int id, string activityValue)
    {
        await _patientService.UpdatePatientActivity(id, activityValue);
        return Ok();
    }
}


    