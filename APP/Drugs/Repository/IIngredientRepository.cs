public interface IIngredientRepository
{
    public Task<DrugIngredients> GetAllIngredients();

    public Task InsertIngredient(string ingredientName);

    public Task UpdateIngredientInformation(string oldIngredient, string newIngredient);

    public Task DeleteIngredient(string name);
}