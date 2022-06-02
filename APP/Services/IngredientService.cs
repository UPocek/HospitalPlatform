public class IngredientService : IIngredientService
{
    private IIngredientRepository repository;

    public IngredientService()
    {
        repository = new IngredientRepository();
    }

    public async Task<DrugIngredients> GetAllIngredients()
    {
        return await repository.GetAllIngredients();
    }
}