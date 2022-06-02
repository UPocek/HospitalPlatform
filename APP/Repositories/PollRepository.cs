using MongoDB.Driver;


public class PollRepository : IPollRepository
{

    private IMongoDatabase database;

    public PollRepository()
    {

        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        database = client.GetDatabase("USI");

    }

    public async Task<Hospital> GetHospital()
    {
        var hospital = database.GetCollection<Hospital>("Hospital");
        return await hospital.Find(item => true).FirstOrDefaultAsync();
    }

}