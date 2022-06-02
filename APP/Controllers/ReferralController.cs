using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ReferralController : ControllerBase
{
    private IReferralService service;
    private string dateToday;

    public ReferralController()
    {
        service = new ReferralService();

        dateToday = DateTime.Now.ToString("yyyy-MM-dd");
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> CreateReferralForPatient(int id, Referral referral)
    {
        var referralId = await service.FindReferralId(id);
        referral.ReferralId = referralId;
        await service.CreateReferralForPatient(id, referral);
        return Ok();
    }
}