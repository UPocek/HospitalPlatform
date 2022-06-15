using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ScheduleController : ControllerBase
{
    private IScheduleService _scheduleService;
    private IValidateExaminationService _validateExaminationService;

    public ScheduleController()
    {
        _scheduleService = new ScheduleService();
        _validateExaminationService = new ValidateExaminationService();
    }

    //get all examintions in database
    [HttpGet("")]
    public async Task<List<Examination>> GetAllExaminations()
    {
        return await _scheduleService.GetAllExaminations();
    }

    //get examination based on id
    [HttpGet("{id}")]
    public async Task<Examination> GetExamination(int id)
    {
        return await _scheduleService.GetExamination(id);
    }

    //get all examinations related to current doctor
    [HttpGet("doctor/{doctorId}")]
    public async Task<List<Examination>> GetAllDoctorsExaminations(int doctorId)
    {
        return await _scheduleService.GetAllDoctorsExaminations(doctorId);
    }

    [HttpGet("doctorSchedule/{doctorId}&{dateOfSearch}")]
    public async Task<List<Examination>> GetAllDoctorsExaminationsSchedule(int doctorId, string dateOfSearch)
    {
        return await _scheduleService.GetDoctorsExaminationsSchedule(doctorId, dateOfSearch);
    }

    //get all examinations related to current patient
    [HttpGet("patient/{patientId}")]
    public async Task<List<Examination>> GetAllPatientsExaminations(int patientId)
    {
        return await _scheduleService.GetAllPatientsExaminations(patientId);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExamination(int id, Examination examination)
    {
        if (!await _validateExaminationService.IsUpdatedExaminationValid(examination))
        {
            return BadRequest();
        }

        await _scheduleService.UpdateExamination(id, examination);

        return Ok();
    }

    //prebaci u referrals
    [HttpPost("reffered/{specialization}/{referralid}")]
    public async Task<IActionResult> CreateRefferedExamination(Examination newExamination, string specialization, int referralid)
    {

        var roomValidation = await _validateExaminationService.IsRoomValid(newExamination.RoomName);
        var doctorValidation = await _scheduleService.doesSpecializedDoctorExist(newExamination.DoctorId, specialization);
        var durationValidation = _scheduleService.isDurationValid(newExamination);

        if (!(roomValidation && doctorValidation && durationValidation))
        {
            return BadRequest();
        }

        await _scheduleService.CreateRefferedExamination(newExamination, specialization, referralid);
        return Ok();
    }


    [HttpPost("urgent/{specialization}")]
    public async Task<List<Examination>> CreateUrgentExamination(Examination newExamination, string specialization)
    {
        return await _scheduleService.CreateUrgentExamination(newExamination, specialization);
    }

    [HttpPost("urgent/termMoving")]
    public async Task<IActionResult> CreateUrgentExaminationWithTermMoving(Examination newExamination)
    {
        await _scheduleService.CreateUrgentExaminationWithTermMoving(newExamination);
        return Ok();
    }

    [HttpPost("new")]
    public async Task<IActionResult> CreateExamination(Examination examination)
    {
        if (!await _validateExaminationService.IsNewExaminationValid(examination))
        {
            return BadRequest();
        }

        await _scheduleService.SaveExamination(examination);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExamination(int id)
    {
        await _scheduleService.DeleteExamination(id);

        return Ok();
    }

}