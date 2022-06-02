using MongoDB.Driver;

public class RenovationRepository : IRenovationRepository
{
    private IMongoDatabase _database;

    public RenovationRepository()
    {
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        _database = client.GetDatabase("USI");
    }

    public async Task<List<Renovation>> GetAllRenovations()
    {
        var renovations = _database.GetCollection<Renovation>("Renovations");
        return await renovations.Find(renovation => true).ToListAsync();
    }

    public async Task<List<Renovation>> GetAllRenovationsInRoom(string roomName)
    {
        var renovations = _database.GetCollection<Renovation>("Renovations");
        return await renovations.Find(item => item.Room == roomName).ToListAsync();
    }

    public async Task InsertRenovation(Renovation renovation)
    {
        var renovations = _database.GetCollection<Renovation>("Renovations");
        await renovations.InsertOneAsync(renovation);
    }

    public async Task StartRenovationInRoom(string roomName)
    {
        var rooms = _database.GetCollection<Room>("Rooms");
        var update = Builders<Room>.Update.Set("inRenovation", true);
        await rooms.UpdateManyAsync(item => item.Name == roomName, update);
    }

}