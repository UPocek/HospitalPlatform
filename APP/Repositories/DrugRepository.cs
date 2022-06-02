using MongoDB.Driver;

public class DrugRepository : IDrugRepository
{
    private IMongoDatabase database;

    public DrugRepository()
    {

        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        database = client.GetDatabase("USI");

    }

    public async Task<List<Drug>> GetAllDrugs()
    {
        var drugs = database.GetCollection<Drug>("Drugs");
        return await drugs.Find(item => true).ToListAsync();
    }

    public async Task<List<Drug>> GetAllDrugsForReview()
    {
        var drugs = database.GetCollection<Drug>("Drugs");
        return await drugs.Find(item => item.Status == "inReview").ToListAsync();
    }

    public async Task<List<string>> GetDrugsIngredients(string name)
    {
        var drugs = database.GetCollection<Drug>("Drugs");
        var drug = await drugs.Find(item => item.Name == name).FirstOrDefaultAsync();
        return drug.Ingredients;
    }

    public async Task UpdateDrugMessage(string id, Dictionary<string, string> data)
    {
        var drugs = database.GetCollection<Drug>("Drugs");
        var filter = Builders<Drug>.Filter.Eq("name", id);
        var update = Builders<Drug>.Update.Set("comment", data["message"]);
        await drugs.UpdateOneAsync(filter, update);
    }

    public async Task ApproveDrug(string id)
    {
        var drugs = database.GetCollection<Drug>("Drugs");
        var filter = Builders<Drug>.Filter.Eq("name", id);
        var update = Builders<Drug>.Update.Set("status", "inUse");
        await drugs.UpdateOneAsync(filter, update);

    }
}