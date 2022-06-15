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

    public async Task<List<Examination>> GetAvailableExamination(ExaminationFilter filter)
    {
        var dueDate = DateTime.Parse(filter.dueDate);
        var timeFrom = DateTime.Parse(filter.timeFrom);
        var timeTo = DateTime.Parse(filter.timeTo);
        var intervalFrom = DateTime.Today.AddDays(1).AddHours(timeFrom.Hour).AddMinutes(timeFrom.Minute);
        var intervalEnd = DateTime.Today.AddDays(1).AddHours(timeTo.Hour).AddMinutes(timeTo.Minute);

        var examinations = _database.GetCollection<Examination>("MedicalExaminations");
        var doctorsExaminations = examinations.Find(e => e.DoctorId == filter.doctorId).ToList();
        var patientsExaminations = examinations.Find(e => e.PatinetId == filter.patientId).ToList();

        var result = new List<Examination>();
        var examinationTime = intervalFrom;

        while (examinationTime < dueDate)
        {

            Examination examination = new Examination();
            examination.DoctorId = filter.doctorId;
            examination.DateAndTimeOfExamination = examinationTime.ToString("yyyy-MM-ddTHH:mm");
            examination.TypeOfExamination = "visit";

            //check if doctor is free 
            bool isDoctorAvalialbe = checkAdvancedAvilability(doctorsExaminations, examinationTime);
            //check if patient is free
            bool isPatientAvalilable = checkAdvancedAvilability(patientsExaminations, examinationTime);

            if (isDoctorAvalialbe && isPatientAvalilable)
            {
                result.Add(examination);
            }
            //continue with finding available examinations  
            examinationTime = examinationTime.AddMinutes(15);
            if (examinationTime.AddMinutes(15) > intervalEnd)
            {
                intervalFrom = intervalFrom.AddDays(1);
                intervalEnd = intervalEnd.AddDays(1);
                examinationTime = intervalFrom;
            }
        }

        return result;
    }


    public async Task UpdateExamination(int id, Examination examination)
    {
        var examinations = _database.GetCollection<Examination>("MedicalExaminations");
        await examinations.FindOneAndReplaceAsync(e => e.Id == id, examination);
    }

    public async Task UpdatePatientsExamination(string id,Examination examination)
    {
        var examinations = _database.GetCollection<Examination>("MedicalExaminations");
        DateTime examinationBegin = DateTime.Parse(examination.DateAndTimeOfExamination);
        DateTime examinationEnd = examinationBegin.AddMinutes(examination.DurationOfExamination);
        Examination examination1 = examination;

        var patients = _database.GetCollection<Patient>("Patients");
        Patient patient = patients.Find(p => p.Id == examination.PatinetId).FirstOrDefault();

        var patientsExaminations = examinations.Find(item => item.PatinetId == examination.PatinetId).ToList();
        bool isPatientAvalilable = checkAvailability(patientsExaminations, examinationBegin, examinationEnd);

        bool isTroll = TrollCheck(patient, "deleted/updated", 5);

        var doctorsExaminations = examinations.Find(item => item.DoctorId == examination.DoctorId).ToList();
        bool isDoctorAvalialbe = checkAvailability(doctorsExaminations, examinationBegin, examinationEnd);

        if (!(isPatientAvalilable && isDoctorAvalialbe && isTroll))
        {
            return;
        }

        var oldExaminationData = examinations.Find(item => item.Id == int.Parse(id)).FirstOrDefault();
        examination._id = oldExaminationData._id;
        examination.Id = oldExaminationData.Id;

        var rooms = _database.GetCollection<Room>("Rooms");
        var validRooms = rooms.Find(room => room.InRenovation == false && room.Type == "examination room").ToList();

        foreach (var room in validRooms)
        {
            var examinationsInRoom = examinations.Find(item => item.RoomName == room.Name && item.DateAndTimeOfExamination != examination.DateAndTimeOfExamination).ToList();
            if (examinationsInRoom != null)
            {
                examination.RoomName = room.Name;
                break;
            }

        }
        if (isForRequest(oldExaminationData))
        {
            ExaminationRequest request = new ExaminationRequest();
            request.Examination = examination;
            request.Status = 1;
            await CreateRequest(request);
            return;
        }

        await examinations.FindOneAndReplaceAsync(e => e.Id == int.Parse(id), examination);
        UpdateExaminationHistory("updated", patients, patient.Id);

    }



    public async Task InsertExamination(Examination examination)
    {
        var examinations = _database.GetCollection<Examination>("MedicalExaminations");
        var id = examinations.Find(e => true).SortByDescending(e => e.Id).FirstOrDefault().Id;
        examination.Id = id + 1;
        await examinations.InsertOneAsync(examination);
    }

    public async Task CreateExamination(Examination examination)
    {
        var examinations = _database.GetCollection<Examination>("MedicalExaminations");
        DateTime examinationBegin = DateTime.Parse(examination.DateAndTimeOfExamination);
        DateTime examinationEnd = examinationBegin.AddMinutes(examination.DurationOfExamination);

        var patients = _database.GetCollection<Patient>("Patients");
        Patient patient = patients.Find(p => p.Id == examination.PatinetId).FirstOrDefault();

        var patientsExaminations = examinations.Find(item => item.PatinetId == examination.PatinetId).ToList();
        bool isPatientAvalilable = checkAvailability(patientsExaminations, examinationBegin, examinationEnd);

        bool isTroll = TrollCheck(patient, "deleted/updated", 5);

        var doctorsExaminations = examinations.Find(item => item.DoctorId == examination.DoctorId).ToList();
        bool isDoctorAvalialbe = checkAvailability(doctorsExaminations, examinationBegin, examinationEnd);

        if (!(isPatientAvalilable && isDoctorAvalialbe && isTroll))
        {
            return;
        }


        var rooms = _database.GetCollection<Room>("Rooms");
        var validRooms = rooms.Find(room => room.InRenovation == false && room.Type == "examination room").ToList();
        if (examination.DoctorId % 2 != 0)
        {
            examination.RoomName = validRooms[0].Name;
        }
        else
        {
            examination.RoomName = validRooms[validRooms.Count - 1].Name;
        }

        var id = examinations.Find(e => true).SortByDescending(e => e.Id).FirstOrDefault().Id;
        examination.Id = id + 1;
        examinations.InsertOne(examination);

        UpdateExaminationHistory("created", patients, patient.Id);
    }

    public async Task DeleteExamination(int id)
    {
        var examinations = _database.GetCollection<Examination>("MedicalExaminations");
        await examinations.DeleteOneAsync(e => e.Id == id);
    }

    public async Task DeletePatientsExamination(int id)
    {
        var examinations = _database.GetCollection<Examination>("MedicalExaminations");

        Examination examination = examinations.Find(item => item.Id == id).FirstOrDefault();
        var patients = _database.GetCollection<Patient>("Patients");
        Patient patient = patients.Find(p => p.Id == examination.PatinetId).FirstOrDefault();

        var isTroll = TrollCheck(patient, "deleted/updated", 5);
        if(!isTroll){
            return;
        }

        if(isForRequest(examination)){
            ExaminationRequest request = new ExaminationRequest();
            request.Examination = examination;
            request.Status = 0;
            await CreateRequest(request);
            return;
        }
        await examinations.DeleteOneAsync(e => e.Id == id);
        UpdateExaminationHistory("deleted", patients, patient.Id);
        
    }

    public async Task<List<Examination>> GetExaminationsAfterNow(Examination examination){

        var dateFilter = Builders<Examination>.Filter.Gt(e => e.DateAndTimeOfExamination, DateTime.Now.ToString("yyyy-MM-ddTHH:mm"));
        var roomFilter = Builders<Examination>.Filter.Eq(e => e.RoomName, examination.RoomName);
        var doctorFilter = Builders<Examination>.Filter.Eq(e => e.DoctorId, examination.DoctorId);
        var patientFilter = Builders<Examination>.Filter.Eq(e => e.PatinetId, examination.PatinetId);

        var filter = dateFilter & roomFilter & doctorFilter & patientFilter;

        return await _database.GetCollection<Examination>("MedicalExaminations").Find(filter).SortBy(e => e.DateAndTimeOfExamination).ToListAsync();
    }

    private bool TrollCheck(Patient patient, String type, int n){
        var patients = _database.GetCollection<Patient>("Patients");
        DateTime checkDate = DateTime.Today.AddDays(-30);
        int counter = 0;
        foreach(var entry in patient.ExaminationHistory){
            DateTime entryDate = DateTime.Parse(entry.Date);
            if (entryDate > checkDate && type.Contains(entry.Type)){
                counter++;
            }
            if (entryDate < checkDate){
                var update = Builders<Patient>.Update.PopFirst("examinationHistory");
                patients.UpdateOne(p => p.Id == patient.Id, update);
            }
        }

        if (counter>n){
            var block = Builders<Patient>.Update.Set("active", "2");
            patients.UpdateOne(p => p.Id == patient.Id, block);
            return false;
        }
        return true;
    }

    private bool isForRequest(Examination oldExaminationData){
        DateTime dt = DateTime.Today;
        DateTime dtOfExamination = DateTime.Parse(oldExaminationData.DateAndTimeOfExamination);
        return dt.AddDays(2) >= dtOfExamination;
    }

    private async Task CreateRequest(ExaminationRequest request)
    {
        var requests = _database.GetCollection<ExaminationRequest>("ExaminationRequests");
        await requests.InsertOneAsync(request);
    }

    private void UpdateExaminationHistory(string type, IMongoCollection<Patient> patients, int id){
        ExaminationHistoryEntry newEntry = new ExaminationHistoryEntry();
        newEntry.Date = DateTime.Today.ToString("yyyy-MM-ddTHH:mm");
        newEntry.Type = type;
        var update = Builders<Patient>.Update.Push("examinationHistory", newEntry);
        patients.UpdateOne(p => p.Id == id, update);
    }

    private bool checkAvailability(List<Examination> examinations, DateTime examinationBegin, DateTime examinationEnd)
    {
        foreach (var item in examinations)
        {
            DateTime itemBegin = DateTime.Parse(item.DateAndTimeOfExamination);
            DateTime itemEnd = itemBegin.AddMinutes(item.DurationOfExamination);
            if (examinationBegin >= itemBegin && examinationBegin <= itemEnd || examinationEnd >= itemBegin && examinationEnd <= itemEnd)
            {
                return false;
            }
        }
        return true;
    }

    private bool checkAdvancedAvilability(List<Examination> examinations, DateTime examinationTime)
    {
        foreach (var item in examinations)
        {
            DateTime itemBegin = DateTime.Parse(item.DateAndTimeOfExamination);
            DateTime itemEnd = itemBegin.AddMinutes(item.DurationOfExamination);
            if (examinationTime >= itemBegin && examinationTime <= itemEnd || examinationTime.AddMinutes(15) >= itemBegin && examinationTime.AddMinutes(15) <= itemEnd)
            {
                return false;
            }
        }
        return true;
    }

}