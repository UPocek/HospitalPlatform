using MongoDB.Driver;

public class IngredientRepository : IIngredientRepository
{

    private IMongoDatabase database;

    public IngredientRepository()
    {

        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        database = client.GetDatabase("USI");

    }

    public async Task<DrugIngredients> GetAllIngredients()
    {
        var drugIngredients = database.GetCollection<DrugIngredients>("DrugIngredients");
        return await drugIngredients.Find(item => true).FirstOrDefaultAsync();
    }

}