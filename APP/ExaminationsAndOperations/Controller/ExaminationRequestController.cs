using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ExaminationRequestController : ControllerBase
{
    private IExaminationRequestService _examinationRequestService;

    public ExaminationRequestController()
    {
        _examinationRequestService = new ExaminationRequestService();
    }

    [HttpGet("")]
    public async Task<List<ExaminationRequest>> GetAllExaminations()
    {
        return await _examinationRequestService.GetExaminationRequests();
    }

    [HttpPut("accept/{id}")]
    public async Task<IActionResult> AcceptExaminationRequest(string id)
    {
        await _examinationRequestService.AcceptExaminationRequest(id);
        return Ok();
    }

    [HttpPut("decline/{id}")]
    public async Task<IActionResult> DeclineExaminationRequest(string id)
    {
        await _examinationRequestService.DeclineExaminationRequest(id);
        return Ok();
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateRequest(ExaminationRequest request)
    {
        await _examinationRequestService.CreateRequest(request);
        return Ok();
    }

}