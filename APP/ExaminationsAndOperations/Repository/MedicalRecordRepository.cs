using MongoDB.Driver;

public class MedicalRecordRepository : IMedicalRecordRepository
{
    private IMongoDatabase _database;

    public MedicalRecordRepository()
    {
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        _database = client.GetDatabase("USI");
    }

    public async Task UpdateMedicalRecord(int id, MedicalRecord medicalRecord)
    {
        var patients = _database.GetCollection<Patient>("Patients");
        var updatePatients = Builders<Patient>.Update.Set("medicalRecord", medicalRecord);
        await patients.UpdateOneAsync(p => p.Id == id, updatePatients);
    }

    public async Task UpdatePatientsPerscriptionList(int id, Prescription prescription)
    {
        var patients = _database.GetCollection<Patient>("Patients");
        var updatePatients = Builders<Patient>.Update.AddToSet("perscription", prescription);
        await patients.UpdateOneAsync(p => p.Id == id, updatePatients);
    }
    public async Task<List<MedicalInstruction>> GetPrescriptions(int id)
    {

        var patients = _database.GetCollection<Patient>("Patients");
        Patient patient = patients.Find(e => e.Id == id).FirstOrDefault();
        List<MedicalInstruction> medicalInstructions = new List<MedicalInstruction>();

        foreach(MedicalInstruction instruction in patient.MedicalRecord.MedicalInstructions){
            if(checkDate(instruction)){
                medicalInstructions.Add(instruction);
            }
        }
        
        return medicalInstructions;
    }

    public bool checkDate(MedicalInstruction instruction){
        var startDate = DateTime.Parse(instruction.StartDate) ;
        var endDate = DateTime.Parse(instruction.EndtDate) ;
        DateTime today = DateTime.Today;

        if(startDate <=today && today <= endDate){
            return true;
        }

        return false;
    }
    public async Task<Prescription> GetPrescription(string drug, int id)
    {

        var patients = _database.GetCollection<Patient>("Patients");
        Patient patient = patients.Find(e => e.Id == id).FirstOrDefault();
        var prescriptions = patient.MedicalRecord.Prescriptions;        

    

        return prescriptions.Find(item => item.DrugName == drug);
    }

}