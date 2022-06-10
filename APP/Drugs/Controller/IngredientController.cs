#nullable disable
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class IngredientController : ControllerBase
{
    private IIngredientService _ingredientService;

    public IngredientController()
    {
        _ingredientService = new IngredientService();
    }

    [HttpGet("")]
    public async Task<DrugIngredients> GetAllIngredients()
    {
        return await _ingredientService.GetAllIngredients();
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateIngredinet(Dictionary<string, string> ingredient)
    {
        if (await _ingredientService.IngredientAlreadyExists(ingredient["name"]))
        {
            return BadRequest();

        }

        await _ingredientService.SaveIngredient(ingredient["name"]);

        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIngredinet(string id, Dictionary<string, string> data)
    {
        if (await _ingredientService.IngredientAlreadyExists(data["name"]))
        {
            return BadRequest();
        }

        await _ingredientService.UpdateIngredient(id, data["name"]);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIngredient(string id)
    {
        await _ingredientService.DeleteIngredient(id);

        return Ok();
    }

}