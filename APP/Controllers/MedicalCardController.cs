using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class MedicalCardController : ControllerBase
{
    private IMedicalCardService service;
    private string dateToday;

    public MedicalCardController()
    {
        service = new MedicalCardService();

        dateToday = DateTime.Now.ToString("yyyy-MM-dd");
    }

    [HttpGet("{id}")]
    public async Task<MedicalCard> GetPatientMedicalCard(int id)
    {
        return await service.GetMedicalCardByPatient(id);
    }

}