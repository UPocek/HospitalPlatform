using MongoDB.Driver;

public class DoctorRepository : IDoctorRepository
{
    private IMongoDatabase _database;

    public DoctorRepository()
    {
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        _database = client.GetDatabase("USI");
    }

    public async Task<List<PollForDoctors>> GetAllDoctors()
    {
        var employees = _database.GetCollection<PollForDoctors>("Employees");
        return await employees.Find(item => item.Role == "doctor").ToListAsync();
    }

}