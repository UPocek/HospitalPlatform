using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
public class FreeDaysController : ControllerBase
{
    private IFreeDaysService _freeDaysService;

    public FreeDaysController()
    {
        _freeDaysService = new FreeDaysService();
    }

    [HttpGet("requests")]
    public async Task<List<FreeDayRequest>> GetAllFreeDaysRequests()
    {
        return await _freeDaysService.GetAllFreeDaysRequests();
    }

    [HttpGet("{doctorId}")]
    public async Task<List<FreeDayRequest>> GetAllDoctorsFreeDaysRequests(int doctorId)
    {
        return await _freeDaysService.GetAllDoctorsFreeDaysRequests(doctorId);
    }

    [HttpPost("request")]
    public async Task<IActionResult> SendDoctorsRequest(FreeDayRequest freeDayRequest)
    {
        if (await _freeDaysService.SendDoctorsRequest(freeDayRequest))
        {
            return Ok();
        }
        return BadRequest();
    }

}