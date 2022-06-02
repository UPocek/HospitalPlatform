using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class MedicalRecordController : ControllerBase
{
    private IMedicalRecordService service;
    private string dateToday;

    public MedicalRecordController()
    {
        service = new MedicalRecordService();

        dateToday = DateTime.Now.ToString("yyyy-MM-dd");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMedicalRecord(int id, MedicalRecord medicalRecord )
    {
        await service.UpdateMedicalRecord(id, medicalRecord);
        return Ok();    
    }

    [HttpPut("persrciption/{id}")]
    public async Task<IActionResult> AddPerscriptionToMedicalRecord(int id, Prescription prescription)
    {
        if(await service.IsPerscriptionValid(id, prescription))
        {
            await service.UpdatePatientsPerscriptionList(id, prescription);
            return Ok();  
        }
        return BadRequest();
          
    }

}