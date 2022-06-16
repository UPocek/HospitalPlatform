using MongoDB.Driver;

public class DrugRepository : IDrugRepository
{
    private IMongoDatabase _database;

    public DrugRepository()
    {
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        _database = client.GetDatabase("USI");
    }

    public async Task<List<Drug>> GetAllDrugs()
    {
        var drugs = _database.GetCollection<Drug>("Drugs");
        return await drugs.Find(item => true).ToListAsync();
    }

    public async Task<Drug> GetDrugByName(string name)
    {
        var drugs = _database.GetCollection<Drug>("Drugs");
        return await drugs.Find(item => item.Name == name).FirstOrDefaultAsync();
    }

    public async Task<List<Drug>> GetAllDrugsForReview()
    {
        var drugs = _database.GetCollection<Drug>("Drugs");
        return await drugs.Find(item => item.Status == "inReview").ToListAsync();
    }

    public async Task<List<string>> GetDrugIngredients(string name)
    {
        var drugs = _database.GetCollection<Drug>("Drugs");
        var drug = await drugs.Find(item => item.Name == name).FirstOrDefaultAsync();
        return drug.Ingredients;
    }

    public async Task InsertDrug(Drug drug)
    {
        var drugs = _database.GetCollection<Drug>("Drugs");
        await drugs.InsertOneAsync(drug);
    }

    public async Task DeleteDrug(string name)
    {
        var drugs = _database.GetCollection<Drug>("Drugs");
        await drugs.DeleteOneAsync(item => item.Name == name);
    }

    public async Task UpdateDrugInformation(string nameOfDrugToUpdate, string name, List<string> ingredients, string status)
    {
        var drugs = _database.GetCollection<Drug>("Drugs");

        var filter = Builders<Drug>.Filter.Eq("name", nameOfDrugToUpdate);

        var updateIngredients = Builders<Drug>.Update.Set("ingredients", ingredients);
        await drugs.UpdateOneAsync(filter, updateIngredients);

        var updateName = Builders<Drug>.Update.Set("name", name);
        await drugs.UpdateOneAsync(filter, updateName);

        var updateStatus = Builders<Drug>.Update.Set("status", status);
        await drugs.UpdateOneAsync(filter, updateStatus);
    }

    public async Task UpdateDrugMessage(string id, Dictionary<string, string> data)
    {
        var drugs = _database.GetCollection<Drug>("Drugs");
        var filter = Builders<Drug>.Filter.Eq("name", id);
        var update = Builders<Drug>.Update.Set("comment", data["message"]);
        await drugs.UpdateOneAsync(filter, update);
    }

    public async Task ApproveDrug(string id)
    {
        var drugs = _database.GetCollection<Drug>("Drugs");
        var filter = Builders<Drug>.Filter.Eq("name", id);
        var update = Builders<Drug>.Update.Set("status", "inUse");
        await drugs.UpdateOneAsync(filter, update);
    }

    public async Task CreateNotification(DrugNotification notification)
    {
        var notifications = _database.GetCollection<DrugNotification>("Notifications");
        await notifications.InsertOneAsync(notification);

    }

}