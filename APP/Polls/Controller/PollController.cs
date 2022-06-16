#nullable disable
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class PollController : ControllerBase
{
    private IPollService _pollService;

    public PollController()
    {
        _pollService = new PollService();
    }

    [HttpGet("hospital")]
    public async Task<Hospital> GetHospitalPolls()
    {
        return await _pollService.GetHospitalPolls();
    }

    [HttpGet("doctor")]
    public async Task<List<PollForDoctors>> GetDoctorPolls()
    {
        return await _pollService.GetDoctorPolls();
    }

    [HttpPost("")]
    public async Task<IActionResult> PostPoll(Poll poll)
    {
        await _pollService.PostPoll(poll);
        return Ok();
    }

}