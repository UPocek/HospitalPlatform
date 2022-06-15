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
        Patient resultingPatient = await patients.Find(p => p.Id == id).FirstOrDefaultAsync();
        return resultingPatient;
    }

    public async Task<List<Patient>> GetUnblockedPatients()
    {
        var patients = _database.GetCollection<Patient>("Patients");

        return await patients.Find(item => item.Active == "0").ToListAsync();
    }

    public async Task<Patient> GetUnblockedPatient(int id)
    {
        var patients = _database.GetCollection<Patient>("Patients");

        return await patients.Find(item => item.Id == id && item.Active == "0").FirstOrDefaultAsync();
    }

    public async Task<List<Patient>> GetBlockedPatients()
    {
        var patients = _database.GetCollection<Patient>("Patients");

        return await patients.Find(item => item.Active != "0").ToListAsync();
    }
    public async Task<String> GetPatientActivity(int id)
    {
        var patients = _database.GetCollection<Patient>("Patients"); ;

        var patient = await patients.Find(p => p.Id == id).FirstOrDefaultAsync();

        return patient.Active;
    }

    public async Task CreatePatient(Patient patient)
    {
        var patients = _database.GetCollection<Patient>("Patients");

        Random rnd = new Random();
        patient.Id = rnd.Next(901, 10000);

        // If patient with that id already exists generate another
        do
        {
            patient.Id = rnd.Next(901, 10000);
        }
        while (await PatientExists(patient.Id));

        await patients.InsertOneAsync(patient);
    }

    public async Task UpdatePatient(int id, Patient patient)
    {
        var patients = _database.GetCollection<Patient>("Patients");
        Patient updatedPatient = await patients.Find(p => p.Id == id).FirstOrDefaultAsync();

        updatedPatient.FirstName = patient.FirstName;
        updatedPatient.LastName = patient.LastName;
        updatedPatient.Email = patient.Email;
        updatedPatient.Password = patient.Password;
        updatedPatient.MedicalRecord.Weight = patient.MedicalRecord.Weight;
        updatedPatient.MedicalRecord.Height = patient.MedicalRecord.Height;
        updatedPatient.MedicalRecord.BloodType = patient.MedicalRecord.BloodType;

        await patients.ReplaceOneAsync(p => p.Id == id, updatedPatient);
    }

    public async Task DeletePatient(int id)
    {
        var patients = _database.GetCollection<Patient>("Patients");
        await patients.DeleteOneAsync(p => p.Id == id);

        var examinations = _database.GetCollection<Examination>("MedicalExaminations");
        var filter = Builders<Examination>.Filter.Lt(e => e.DateAndTimeOfExamination, DateTime.Now.ToString("yyyy-MM-ddTHH:mm")) & Builders<Examination>.Filter.Eq("patient", id);
        await examinations.DeleteManyAsync(filter);

    }

    public async Task UpdatePatientActivity(int id, string activityValue)
    {
        var patients = _database.GetCollection<Patient>("Patients");
        Patient updatedPatient = patients.Find(p => p.Id == id).FirstOrDefault();

        updatedPatient.Active = activityValue;

        await patients.ReplaceOneAsync(p => p.Id == id, updatedPatient);
    }

    public async Task<bool> PatientExists(int id)
    {
        var patients = _database.GetCollection<Patient>("Patients");

        if (await patients.Find(p => p.Id == id && p.Active == "0").CountDocumentsAsync() == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

}