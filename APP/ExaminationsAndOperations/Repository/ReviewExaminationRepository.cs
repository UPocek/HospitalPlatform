using MongoDB.Driver;

public class ReviewExaminationRepository : IReviewExaminationRepository
{
    private IMongoDatabase _database;

    public ReviewExaminationRepository()
    {
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        _database = client.GetDatabase("USI");
    }

    public async Task AddPerscription(int id, Prescription prescription)
    {
        var patients = _database.GetCollection<Patient>("Patients");
        var updatePatients = Builders<Patient>.Update.Set("medicalRecord.drugs", prescription);
        await patients.UpdateOneAsync(p => p.Id == id, updatePatients);
    }

    public async Task AddMedicalInstruction(int id, MedicalInstruction medicalInstruction)
    {
        var patients = _database.GetCollection<Patient>("Patients");
        var updatePatients = Builders<Patient>.Update.AddToSet("medicalRecord.medicalInstructioin", medicalInstruction);
        await patients.UpdateOneAsync(p => p.Id == id, updatePatients);
    }

    public async Task UpdatePatientsPerscriptionList(int id, Prescription prescription)
    {
        var patients = _database.GetCollection<Patient>("Patients");
        var updatePatients = Builders<Patient>.Update.AddToSet("perscription", prescription);
        await patients.UpdateOneAsync(p => p.Id == id, updatePatients);
    }

}