using MongoDB.Driver;

public class MedicalCardRepository : IMedicalCardRepository
{
    private IMongoDatabase database;

    public MedicalCardRepository()
    {

        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        database = client.GetDatabase("USI");

    }

    public async Task<MedicalCard> GetMedicalCardByPatient(int patientId){

        var patients = database.GetCollection<MedicalCard>("Patients");
        return await patients.Find(p => p.Id == patientId).FirstOrDefaultAsync();

    }
}