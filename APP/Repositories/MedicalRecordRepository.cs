using MongoDB.Driver;

public class MedicalRecordRepository : IMedicalRecordRepository
{

    private IMongoDatabase database;

    public MedicalRecordRepository()
    {

        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        database = client.GetDatabase("USI");

    }

    public async Task UpdateMedicalRecord(int id, MedicalRecord medicalRecord)
    {
        var patients = database.GetCollection<Patient>("Patients");
        var updatePatients = Builders<Patient>.Update.Set("medicalRecord", medicalRecord);
        await patients.UpdateOneAsync(p => p.Id == id, updatePatients);
        
    }

    public async Task UpdatePatientsPerscriptionList(int id, Prescription prescription)
    {
        var patients = database.GetCollection<Patient>("Patients");
        var updatePatients = Builders<Patient>.Update.AddToSet("perscription", prescription);
        await patients.UpdateOneAsync(p => p.Id == id, updatePatients);
        
    }
}