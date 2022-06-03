using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ExaminationController : ControllerBase
{
    private IExaminationService _examinationService;

    public ExaminationController()
    {
        _examinationService = new ExaminationService();
    }

    [HttpGet("")]
    public async Task<List<Examination>> GetAllExaminations()
    {
        return await _examinationService.GetAllExaminations();
    }

    [HttpGet("doctor/{doctorId}")]
    public async Task<List<Examination>> GetAllDoctorsExaminations(int doctorId)
    {
        return await _examinationService.GetAllDoctorsExaminations(doctorId);
    }

    [HttpGet("doctorSchedule/{doctorId}")]
    public async Task<List<Examination>> GetAllDoctorsExaminationsSchedule(int doctorId, string dateOfSearch)
    {
        return await _examinationService.GetDoctorsExaminationsSchedule(doctorId, dateOfSearch);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<List<Examination>> GetAllPatientsExaminations(int patientId)
    {
        return await _examinationService.GetAllPatientsExaminations(patientId);
    }

    [HttpPost("new")]
    public async Task<IActionResult> CreateExamination(Examination examination)
    {
        if (!await _examinationService.IsNewExaminationValid(examination))
        {
            return BadRequest();
        }

        await _examinationService.SaveExamination(examination);

        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExamination(int id, Examination examination)
    {
        if (!await _examinationService.IsUpdatedExaminationValid(examination))
        {
            return BadRequest();
        }

        await _examinationService.UpdateExamination(id, examination);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExamination(int id)
    {
        await _examinationService.DeleteExamination(id);

        return Ok();
    }

}