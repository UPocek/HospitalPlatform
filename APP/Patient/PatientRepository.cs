using MongoDB.Driver;

public class PatientRepository : IPatientRepository
{
    private IMongoDatabase _database;

    public PatientRepository()
    {
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        _database = client.GetDatabase("USI");
    }

    public async Task<List<Patient>> GetAllPatients()
    {
        var patinets = _database.GetCollection<Patient>("Patients");
        return await patinets.Find(patinet => true).ToListAsync();
    }

    public async Task<Patient> GetPatientById(int id)
    {
        var patients = _database.GetCollection<Patient>("Patients");
        var resultingPatient = patients.Find(p => p.Id == id).FirstOrDefaultAsync();
        return await resultingPatient;
    }

}