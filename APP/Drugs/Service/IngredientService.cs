public class IngredientService : IIngredientService
{
    private IIngredientRepository _ingredientRepository;

    public IngredientService()
    {
        _ingredientRepository = new IngredientRepository();
    }

    public async Task<DrugIngredients> GetAllIngredients()
    {
        return await _ingredientRepository.GetAllIngredients();
    }

    public async Task<bool> IngredientAlreadyExists(string name)
    {
        var allIngredients = await GetAllIngredients();
        return allIngredients.Ingredients.Contains(name);
    }

    public async Task SaveIngredient(string ingredientName)
    {
        await _ingredientRepository.InsertIngredient(ingredientName);
    }

    public async Task UpdateIngredient(string oldIngredient, string newIngredient)
    {
        await _ingredientRepository.UpdateIngredientInformation(oldIngredient, newIngredient);
    }

    public async Task DeleteIngredient(string ingredientName)
    {
        await _ingredientRepository.DeleteIngredient(ingredientName);
    }

}