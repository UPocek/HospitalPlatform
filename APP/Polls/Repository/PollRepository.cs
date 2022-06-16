using MongoDB.Driver;

public class PollRepository : IPollRepository
{
    private IMongoDatabase _database;

    public PollRepository()
    {
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        _database = client.GetDatabase("USI");
    }

    public async Task PostPoll(Poll poll){
        var polls = _database.GetCollection<Poll>("Polls");
        await polls.InsertOneAsync(poll);
    }

}