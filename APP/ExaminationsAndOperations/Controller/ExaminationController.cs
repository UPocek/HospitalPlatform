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

    [HttpGet("{id}")]
    public async Task<Examination> GetExamination(int id){
        return await _examinationService.GetExamination(id);
    }


    [HttpGet("doctor/{doctorId}")]
    public async Task<List<Examination>> GetAllDoctorsExaminations(int doctorId)
    {
        return await _examinationService.GetAllDoctorsExaminations(doctorId);
    }

    [HttpGet("doctorSchedule/{doctorId}&{dateOfSearch}")]
    public async Task<List<Examination>> GetAllDoctorsExaminationsSchedule(int doctorId, string dateOfSearch)
    {
        Console.Write(await _examinationService.GetDoctorsExaminationsSchedule(doctorId, dateOfSearch));
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

    [HttpPost("reffered/{specialization}/{referralid}")]

    public async Task<IActionResult> CreateRefferedExamination(Examination newExamination, string specialization, int referralid){

        var roomValidation = await _examinationService.doesRoomExist(newExamination.RoomName);
        var doctorValidation = await _examinationService.doesSpecializedDoctorExist(newExamination.DoctorId,specialization);
        var durationValidation = _examinationService.isDurationValid(newExamination);

        if(!(roomValidation && doctorValidation && durationValidation)){
            return BadRequest();
        }

        await _examinationService.CreateRefferedExamination(newExamination,specialization,referralid);
        return Ok();
    }

    [HttpPost("urgent/{specialization}")]
    public async Task<List<Examination>> CreateUrgentExamination(Examination newExamination, string specialization){
        return await _examinationService.CreateUrgentExamination(newExamination,specialization);
    }

    [HttpPost("urgent/termMoving")]
    public async Task<IActionResult> CreateUrgentExaminationWithTermMoving(Examination newExamination){
        await _examinationService.CreateUrgentExaminationWithTermMoving(newExamination);
        return Ok();
    }




}