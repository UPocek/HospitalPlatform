using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class DrugController : ControllerBase
{
    private IDrugService _drugService;

    public DrugController()
    {
        _drugService = new DrugService();
    }

    [HttpGet("")]
    public async Task<List<Drug>> GetAllDrugs()
    {
        return await _drugService.GetAllDrugs();
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateDrug(Drug drug)
    {
        if (!await _drugService.IsDrugNameValid(drug))
        {
            return BadRequest();
        }

        await _drugService.SaveDrug(drug);

        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDrug(string id, Drug drug)
    {
        if (!await _drugService.IsDrugNameValid(drug))
        {
            return BadRequest();
        }

        await _drugService.UpdateDrug(id, drug);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDrug(string id)
    {
        await _drugService.DeleteDrug(id);

        return Ok();
    }

    [HttpGet("review")]
    public async Task<List<Drug>> GetAllDrugsForReview()
    {
        return await _drugService.GetAllDrugsForReview();
    }

    [HttpGet("ingredients/{name}")]
    public async Task<List<string>> GetDrugIngredients(string name)
    {
        return await _drugService.GetDrugIngredients(name);
    }

    [HttpPut("message/{id}")]
    public async Task<IActionResult> UpdateDrugMessage(string id, Dictionary<string, string> data)
    {
        await _drugService.UpdateDrugMessage(id, data);

        return Ok();
    }

    [HttpPut("approve/{id}")]
    public async Task<IActionResult> ApproveDrug(string id)
    {
        await _drugService.ApproveDrug(id);

        return Ok();
    }

    [HttpPost("notifications")]
    public async Task<IActionResult> CreateNotification(DrugNotification notification)
    {
        
        await _drugService.CreateNotification(notification);
        
        return Ok(); 
    }
}