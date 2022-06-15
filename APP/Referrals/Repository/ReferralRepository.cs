using MongoDB.Driver;

public class ReferralRepository : IReferralRepository
{
    private IMongoDatabase _database;

    public ReferralRepository()
    {
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        _database = client.GetDatabase("USI");
    }

    public async Task CreateReferralForPatient(int id, Referral referral)
    {
        var patients = _database.GetCollection<Patient>("Patients");
        var updatePatients = Builders<Patient>.Update.AddToSet("referrals", referral);
        await patients.UpdateOneAsync(p => p.Id == id, updatePatients);
    }

    
    public async void DeletePatientReferral(int referralid, Examination newExamination)
    {
        Patient updatedPatient = await _database.GetCollection<Patient>("Patients").Find( item=>item.Id == newExamination.PatinetId).FirstOrDefaultAsync();

        foreach (Referral patientReferral in updatedPatient.MedicalRecord.Referrals)
        {
            if (patientReferral.ReferralId == referralid)
            {
                updatedPatient.MedicalRecord.Referrals.Remove(patientReferral);
                break;
            }
        }

        await _database.GetCollection<Patient>("Patients").ReplaceOneAsync(p => p.Id == updatedPatient.Id, updatedPatient);
    }

    

}