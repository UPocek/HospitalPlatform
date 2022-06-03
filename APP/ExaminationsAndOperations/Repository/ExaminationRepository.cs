using MongoDB.Driver;

public class ExaminationRepository : IExaminationRepository
{
    private IMongoDatabase _database;

    public ExaminationRepository()
    {
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        _database = client.GetDatabase("USI");
    }

    public async Task<List<Examination>> GetAllExaminations()
    {
        var examinations = _database.GetCollection<Examination>("MedicalExaminations");
        return await examinations.Find(e => true).ToListAsync();
    }

    public async Task<List<Examination>> GetAllExaminationsInRoom(string roomName)
    {
        var medicalExaminations = _database.GetCollection<Examination>("MedicalExaminations");
        return await medicalExaminations.Find(item => item.RoomName == roomName).ToListAsync();
    }

    public async Task<List<Examination>> GetAllDoctorsExaminations(int doctorId)
    {
        Console.Write(doctorId);
        var examinations = _database.GetCollection<Examination>("MedicalExaminations");
        return await examinations.Find(e => e.DoctorId == doctorId).ToListAsync();
    }

    public async Task<List<Examination>> GetAllPatientsExaminations(int patientId)
    {
        var examinations = _database.GetCollection<Examination>("MedicalExaminations");
        return await examinations.Find(e => e.PatinetId == patientId).ToListAsync();
    }

    public async Task UpdateExamination(int id, Examination examination)
    {
        var examinations = _database.GetCollection<Examination>("MedicalExaminations");
        await examinations.FindOneAndReplaceAsync(e => e.Id == id, examination);
    }

    public async Task InsertExamination(Examination examination)
    {
        var examinations = _database.GetCollection<Examination>("MedicalExaminations");
        var id = examinations.Find(e => true).SortByDescending(e => e.Id).FirstOrDefault().Id;
        examination.Id = id + 1;
        await examinations.InsertOneAsync(examination);
    }

    public async Task DeleteExamination(int id)
    {
        var examinations = _database.GetCollection<Examination>("MedicalExaminations");
        await examinations.DeleteOneAsync(e => e.Id == id);
    }

}