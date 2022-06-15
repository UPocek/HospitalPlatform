using MongoDB.Driver;

public class MedicalCardRepository : IMedicalCardRepository
{
    private IMongoDatabase _database;

    public MedicalCardRepository()
    {
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        _database = client.GetDatabase("USI");
    }

    public async Task<MedicalCard> GetMedicalCard(int patientId)
    {
        var patients = _database.GetCollection<MedicalCard>("Patients");
        return await patients.Find(p => p.Id == patientId).FirstOrDefaultAsync();
    }

}