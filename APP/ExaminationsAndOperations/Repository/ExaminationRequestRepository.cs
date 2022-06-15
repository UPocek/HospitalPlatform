using MongoDB.Driver;

public class ExaminationRequestRepository : IExaminationRequestRepository
{
    private IMongoDatabase _database;

    public ExaminationRequestRepository()
    {
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        _database = client.GetDatabase("USI");
    }

    public async Task<List<ExaminationRequest>> GetExaminationRequests()
    {
        var requests = _database.GetCollection<ExaminationRequest>("ExaminationRequests");

        //Delete deprecated requests
        var filter = Builders<ExaminationRequest>.Filter.Lt(e => e.Examination.DateAndTimeOfExamination, DateTime.Now.ToString("yyyy-MM-ddTHH:mm"));
        requests.DeleteMany(filter);

        return await requests.Find(item => true).ToListAsync();
    }

    public async Task AcceptExaminationRequest(string id)
    {
        var requests = _database.GetCollection<ExaminationRequest>("ExaminationRequests");
        ExaminationRequest examinationRequest = requests.Find(e => e._Id == id).FirstOrDefault();

        var examination = examinationRequest.Examination;


        var examinations = _database.GetCollection<Examination>("Examinations");

        if (examinationRequest.Status == 0)
        {
            examinations.DeleteOne(e => e.Id == examination.Id);
        }
        else
        {
            examinations.ReplaceOne(e => e.Id == examination.Id, examination);
        }
        await requests.DeleteOneAsync(e => e._Id == id);
    }


    public async Task DeclineExaminationRequest(string id)
    {
        var requests = _database.GetCollection<ExaminationRequest>("ExaminationRequests");

        await requests.DeleteOneAsync(e => e._Id == id);
    }

}