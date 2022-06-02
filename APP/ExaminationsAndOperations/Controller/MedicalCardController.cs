using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class MedicalCardController : ControllerBase
{
    private IMedicalCardService _medicalCardService;

    public MedicalCardController()
    {
        _medicalCardService = new MedicalCardService();
    }

    [HttpGet("{id}")]
    public async Task<MedicalCard> GetPatientsMedicalCard(int patientId)
    {
        return await _medicalCardService.GetMedicalCard(patientId);
    }

}