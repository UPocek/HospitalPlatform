using MongoDB.Driver;

public class HospitalRepository : IHospitalRepository
{
    private IMongoDatabase _database;

    public HospitalRepository()
    {

        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        _database = client.GetDatabase("USI");

    }

    public async Task<Hospital> GetHospital()
    {
        var hospital = _database.GetCollection<Hospital>("Hospital");
        return await hospital.Find(item => true).FirstOrDefaultAsync();
    }

}