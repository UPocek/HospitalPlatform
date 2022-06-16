#nullable disable
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class RenovationController : ControllerBase
{
    private IRenovationService _renovationService;

    private IScheduleService _scheduleService;

    private string _dateToday;

    public RenovationController()
    {
        _renovationService = new RenovationService();

        _scheduleService = new ScheduleService();

        _dateToday = DateTime.Now.ToString("yyyy-MM-dd");
    }

    [HttpPost("simple")]
    public async Task<IActionResult> CreateSimpleRenovation(Renovation renovation)
    {
        if (await _scheduleService.ExaminationScheduledAtThatTime(renovation) || await _renovationService.RenovationScheduledAtThatTime(renovation))
        {
            return BadRequest();
        }

        await _renovationService.SaveRenovation(renovation);

        if (_dateToday == renovation.StartDate)
        {
            await _renovationService.StartSimpleRenovation(renovation);
        }

        return Ok();
    }

    [HttpPost("devide")]
    public async Task<IActionResult> CreateDevideRenovation(Renovation renovation)
    {
        if (await _scheduleService.ExaminationScheduledAtThatTime(renovation) || await _renovationService.RenovationScheduledAtThatTime(renovation))
        {
            return BadRequest();
        }

        if (_dateToday == renovation.StartDate)
        {
            await _renovationService.StartDevideRenovation(renovation);
        }
        else
        {
            await _renovationService.SaveRenovation(renovation);
        }

        return Ok();
    }

    [HttpPost("merge")]
    public async Task<IActionResult> CreateMergeRenovation(Renovation renovation)
    {
        if (await _scheduleService.ExaminationScheduledAtThatTime(renovation) || await _renovationService.RenovationScheduledAtThatTime(renovation))
        {
            return BadRequest();
        }

        if (_dateToday == renovation.StartDate)
        {
            await _renovationService.StartMergeRenovation(renovation);
        }
        else
        {
            await _renovationService.SaveRenovation(renovation);
        }

        return Ok();
    }

    [HttpGet("")]
    public async Task<List<Renovation>> GetAllRenovations()
    {
        return await _renovationService.GetAllRenovations();
    }

}