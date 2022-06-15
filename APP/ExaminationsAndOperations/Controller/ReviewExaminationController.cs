using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ReviewExaminationController : ControllerBase
{
    private IReviewExaminationService _reviewExaminationService;

    public ReviewExaminationController()
    {
        _reviewExaminationService = new ReviewExaminationService();
    }

    [HttpPut("prescription/{patientId}")]
    public async Task<IActionResult> AddPerscription(int patientId, Prescription prescription)
    {
        await _reviewExaminationService.AddPerscription(patientId, prescription);

        return Ok();
    }

    [HttpPut("medicalinstruction/{patientId}")]
    public async Task<IActionResult> AddMedicalInstruction(int patientId, MedicalInstruction medicalInstruction)
    {
        await _reviewExaminationService.AddMedicalInstruction(patientId, medicalInstruction);

        return Ok();
    }

    [HttpPut("persrciption/{id}")]
    public async Task<IActionResult> AddPerscriptionToMedicalRecord(int id, Prescription prescription)
    {
        if (await _reviewExaminationService.IsPerscriptionValid(id, prescription))
        {
            await _reviewExaminationService.UpdatePatientsPerscriptionList(id, prescription);

            return Ok();
        }

        return BadRequest();
    }

}