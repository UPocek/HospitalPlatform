using MongoDB.Driver;

public class PatientRepository : IPatientRepository
{

    private IMongoDatabase database;

    public PatientRepository()
    {

        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        database = client.GetDatabase("USI");

    }

    public async Task<List<Patient>> GetAllPatients()
    {
        var patinets = database.GetCollection<Patient>("Patients");
        return await patinets.Find(patinet => true).ToListAsync();
    }

    public async Task<Patient> GetPatientById(int id)
    {
        var patients = database.GetCollection<Patient>("Patients");
        var resultingPatient = patients.Find(p => p.Id == id).FirstOrDefaultAsync();
        return await resultingPatient;
    }
}