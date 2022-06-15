using MongoDB.Driver;

public class IngredientRepository : IIngredientRepository
{
    private IMongoDatabase _database;

    public IngredientRepository()
    {
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        _database = client.GetDatabase("USI");
    }

    public async Task<DrugIngredients> GetAllIngredients()
    {
        var drugIngredients = _database.GetCollection<DrugIngredients>("DrugIngredients");
        return await drugIngredients.Find(item => true).FirstOrDefaultAsync();
    }

    public async Task InsertIngredient(string ingredientName)
    {
        var drugIngredients = _database.GetCollection<DrugIngredients>("DrugIngredients");
        var update = Builders<DrugIngredients>.Update.Push("ingredients", ingredientName);
        await drugIngredients.UpdateManyAsync(item => true, update);
    }

    public async Task DeleteIngredient(string name)
    {
        var drugIngredients = _database.GetCollection<DrugIngredients>("DrugIngredients");
        var update = Builders<DrugIngredients>.Update.Pull("ingredients", name);
        await drugIngredients.UpdateManyAsync(item => true, update);
    }

    public async Task UpdateIngredientInformation(string oldIngredient, string newIngredient)
    {
        await DeleteIngredient(oldIngredient);
        await InsertIngredient(newIngredient);
    }

}