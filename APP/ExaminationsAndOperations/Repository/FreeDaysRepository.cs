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

    public async Task DeleteStaleFreeDaysRequests(){
        var freeDayRequests = _database.GetCollection<FreeDayRequest>("DoctorFreeDayRequests");
        foreach (FreeDayRequest fdr in await freeDayRequests.Find(request => request.Status == "waiting").ToListAsync()) {
            string now = DateTime.Now.ToString("yyyy-MM-dd");
            if (String.Compare(fdr.StartDay,now) < 0){
               await freeDayRequests.DeleteOneAsync(request => fdr._Id == request._Id);
            }
        } 
    }

    public async Task DeleteFreeDaysRequest(string id){
        var freeDayRequests = _database.GetCollection<FreeDayRequest>("DoctorFreeDayRequests");
        await freeDayRequests.DeleteOneAsync(request => request._Id == id);
    }

    public async Task<FreeDayRequest> GetFreeDaysRequest(string requestId){
        var freeDayRequests = _database.GetCollection<FreeDayRequest>("DoctorFreeDayRequests");
        return await freeDayRequests.Find(request => request._Id == requestId).FirstOrDefaultAsync();
    }

    public async Task<List<FreeDayRequest>> GetAllFreeDaysRequests()
    {
        var freeDayRequests = _database.GetCollection<FreeDayRequest>("DoctorFreeDayRequests");
        return await freeDayRequests.Find(request => request.Status == "waiting").ToListAsync();

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