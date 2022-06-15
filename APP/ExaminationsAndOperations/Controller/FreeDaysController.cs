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

    [HttpGet("{doctorId}")]
    public async Task<List<FreeDayRequest>> GetAllDoctorsFreeDaysRequests(int doctorId)
    {
        return await _freeDaysService.GetAllDoctorsFreeDaysRequests(doctorId);
    }

}