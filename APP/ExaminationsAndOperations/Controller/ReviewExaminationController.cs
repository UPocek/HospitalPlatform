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

    [HttpGet("prescription/{id}")]
    public async Task<List<MedicalInstruction>> GetPrescriptions(int id)
    {

        return await _reviewExaminationService.GetPrescriptions(id);
    }


    [HttpGet("prescription/{drug}/{id}")]
    public async Task<Prescription> GetPrescription(string drug, int id)
    {

        return await _reviewExaminationService.GetPrescription(drug, id);
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