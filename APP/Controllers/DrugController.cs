using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class DrugController : ControllerBase
{
    private IDrugService service;
    private string dateToday;

    public DrugController()
    {
        service = new DrugService();

        dateToday = DateTime.Now.ToString("yyyy-MM-dd");
    }

    [HttpGet("all")]
    public async Task<List<Drug>> GetAllDrugs()
    {
        return await service.GetAllDrugs();
    }

    [HttpGet("review")]
    public async Task<List<Drug>> GetAllDrugsForReview()
    {
        return await service.GetAllDrugsForReview();
    }

    [HttpGet("ingredients/{id}")]
    public async Task<List<string>> GetDrugsIngredients(string name)
    {
        return await service.GetDrugsIngredients(name);
    }

    [HttpPut("message/{id}")]
    public async Task<IActionResult> UpdateDrugMessage(string id, Dictionary<string, string> data)
    {
       await service.UpdateDrugMessage(id, data);
       return Ok();
    }

    [HttpPut("approve/{id}")]
    public async Task<IActionResult> ApproveDrug(string id)
    {
        await service.ApproveDrug(id);
        return Ok();
    }

}