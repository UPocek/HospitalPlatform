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

    public async Task<Examination> GetExamination(int id)
    {
        var examinations = _database.GetCollection<Examination>("MedicalExaminations");

        return await examinations.Find(item => item.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Examination>> GetAllExaminationsInRoom(string roomName)
    {
        var medicalExaminations = _database.GetCollection<Examination>("MedicalExaminations");
        return await medicalExaminations.Find(item => item.RoomName == roomName).ToListAsync();
    }

    public async Task<List<Examination>> GetAllDoctorsExaminations(int doctorId)
    {
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

    public async Task<List<Examination>> GetExaminationsAfterNow(Examination examination){

        var dateFilter = Builders<Examination>.Filter.Gt(e => e.DateAndTimeOfExamination, DateTime.Now.ToString("yyyy-MM-ddTHH:mm"));
        var roomFilter = Builders<Examination>.Filter.Eq(e => e.RoomName, examination.RoomName);
        var doctorFilter = Builders<Examination>.Filter.Eq(e => e.DoctorId, examination.DoctorId);
        var patientFilter = Builders<Examination>.Filter.Eq(e => e.PatinetId, examination.PatinetId);

        var filter = dateFilter & roomFilter & doctorFilter & patientFilter;

        return await _database.GetCollection<Examination>("MedicalExaminations").Find(filter).SortBy(e => e.DateAndTimeOfExamination).ToListAsync();
    }

}