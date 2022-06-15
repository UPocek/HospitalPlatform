using MongoDB.Driver;

public class FreeDaysRepository : IFreeDaysRepository
{
    private IMongoDatabase _database;

    public FreeDaysRepository()
    {
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        _database = client.GetDatabase("USI");
    }

    public async Task<List<FreeDayRequest>> GetAllFreeDaysRequests()
    {
        var freeDayRequests = _database.GetCollection<FreeDayRequest>("DoctorFreeDayRequests");
        return await freeDayRequests.Find(request => true).ToListAsync();

    }

    public async Task<List<FreeDayRequest>> GetAllDoctorsFreeDaysRequests(int doctorId)
    {
        var freeDayRequests = _database.GetCollection<FreeDayRequest>("DoctorFreeDayRequests");
        return await freeDayRequests.Find(request => request.DoctorId == doctorId).ToListAsync();

    }
    public async Task RequestFreeDays(FreeDayRequest freeDayRequest)
    {
        var freeDayRequests = _database.GetCollection<FreeDayRequest>("DoctorFreeDayRequests");
        await freeDayRequests.InsertOneAsync(freeDayRequest);
    }

}