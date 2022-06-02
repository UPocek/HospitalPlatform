#nullable disable
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class PollController
{

    private IPollService service;

    public PollController()
    {
        service = new PollService();
    }

    [HttpGet("all_hospital")]
    public async Task<Hospital> GetHospitalPolls()
    {
        return await service.GetHospitalPolls();
    }

    [HttpGet("all_doctor")]
    public async Task<List<PollForDoctors>> GetDoctorPolls()
    {
        return await service.GetDoctorPolls();
    }


}