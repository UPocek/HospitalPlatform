using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ExaminationController : ControllerBase
{
    private IExaminationService service;
    private string dateToday;

    public ExaminationController()
    {
        service = new ExaminationService();

        dateToday = DateTime.Now.ToString("yyyy-MM-dd");
    }

    [HttpGet("/all")]
    public async Task<List<Examination>> GetAllExaminations()
    {
        return await service.GetAllExaminations();
    }

    [HttpGet("doctor/{id}")]
    public async Task<List<Examination>> GetAllDoctorsExaminations(int doctorId)
    {
        return await service.GetAllDoctorsExaminations(doctorId);
    }

    [HttpGet("doctorSchedule/{id}")]
    public async Task<List<Examination>> GetAllDoctorsExaminationsSchedule(int doctorId, string dateOfSearch)
    {
        return await service.GetDoctorsExaminationsSchedule(doctorId, dateOfSearch);
    }

    [HttpGet("patient/{id}")]
    public async Task<List<Examination>> GetAllPatientsExaminations(int patientId)
    {
        return await service.GetAllPatientsExaminations(patientId);
    }

    [HttpPost("new")]
    public async Task<IActionResult> CreateNewExamination(Examination examination)
    {
        if(!await service.IsNewExaminationValid(examination))
        {
            return BadRequest();
        }
        await service.SaveExamination(examination);
        return Ok();

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExamination(int id, Examination examination)
    {
        if(!await service.IsUpdatedExaminationValid(examination))
        {
            return BadRequest();
        }
        await service.UpdateExamination(id, examination);
        return Ok();

    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExamination(int id)
    {
        await service.DeleteExamination(id);
        return Ok();
    }
}