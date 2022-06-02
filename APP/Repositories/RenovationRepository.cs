using MongoDB.Driver;

public class RenovationRepository : IRenovationRepository
{
    private IMongoDatabase database;

    public RenovationRepository()
    {

        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        database = client.GetDatabase("USI");

    }
    public async Task<List<Renovation>> GetAllRenovations()
    {
        var renovations = database.GetCollection<Renovation>("Renovations");
        return await renovations.Find(renovation => true).ToListAsync();
    }
}