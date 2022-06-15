using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ReferralController : ControllerBase
{
    private IReferralService _referralService;

    public ReferralController()
    {
        _referralService = new ReferralService();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> CreateReferralForPatient(int patientId, Referral referral)
    {
        var referralId = await _referralService.FindReferralId(patientId);
        referral.ReferralId = referralId;
        await _referralService.CreateReferralForPatient(patientId, referral);
        return Ok();
    }

}