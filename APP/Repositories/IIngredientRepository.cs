public interface IIngredientRepository
{
    public Task<DrugIngredients> GetAllIngredients();
}