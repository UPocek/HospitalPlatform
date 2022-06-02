#nullable disable
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class IngredientController
{

    private IIngredientService service;

    public IngredientController()
    {
        service = new IngredientService();
    }

    [HttpGet("all")]
    public async Task<DrugIngredients> GetIngredients()
    {
        return await service.GetAllIngredients();
    }

}