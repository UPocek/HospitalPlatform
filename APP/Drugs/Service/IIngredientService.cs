public interface IIngredientService
{
    public Task<DrugIngredients> GetAllIngredients();

    public Task<bool> IngredientAlreadyExists(string name);

    public Task SaveIngredient(string ingredientName);

    public Task UpdateIngredient(string oldIngredient, string newIngredient);

    public Task DeleteIngredient(string ingredientName);
}