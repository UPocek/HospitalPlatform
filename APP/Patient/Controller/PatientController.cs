#nullable disable
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net.Mail;

[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private IMongoDatabase _database;

    private IPatientService _patientService;
    public PatientController()
    {
        _patientService = new PatientService();
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        _database = client.GetDatabase("USI");
    }
    //From here SOLID

    [HttpGet("")]
    public async Task<List<Patient>> GetAllPatients()
    {
        return await _patientService.GetAllPatients();
    }

    [HttpGet("{id}")]
    public async Task<Patient> GetPatientById(int id)
    {
        return await _patientService.GetPatientById(id);
    }

    [HttpGet("unblocked")]
    public async Task<List<Patient>> GetUnblockedPatients()
    {
        return await _patientService.GetUnblockedPatients();
    }

    [HttpGet("blocked")]
    public async Task<List<Patient>> GetBlockedPatients()
    {
        return await _patientService.GetBlockedPatients();
    }

    [HttpGet("unblocked/{id}")]
    public async Task<Patient> GetUnblockedPatient(int id)
    {
        return await _patientService.GetUnblockedPatient(id);
    }

    [HttpGet("activity/{id}")]
    public async Task<String> GetPatientActivity(int id)
    {
        return await _patientService.GetPatientActivity(id);
    }
    [HttpPost("create")]
    public async Task<IActionResult> CreatePatient(Patient patient)
    {
        if (!await _patientService.isNewPatientValid(patient))
        {
            return BadRequest();
        }
        await _patientService.CreatePatient(patient);
        return Ok();
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePatient(int id, Patient patient)
    {
        await _patientService.UpdatePatient(id, patient);
        return Ok();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePatient(int id)
    {
        await _patientService.DeletePatient(id);
        return Ok();
    }

    [HttpPut("activity/{id}/{activityValue}")]
    public async Task<IActionResult> UpdatePatientActivity(int id, string activityValue)
    {
        await _patientService.UpdatePatientActivity(id, activityValue);
        return Ok();
    }

    //From here non SOLID

    [HttpGet("doctors")]
    public IActionResult GetAllDoctors()
    {
        var collection = _database.GetCollection<BsonDocument>("Employees");
        var filter = Builders<BsonDocument>.Filter.Eq("role", "doctor");
        var results = collection.Find(filter).ToList();
        var dotNetObjList = results.ConvertAll(BsonTypeMapper.MapToDotNetValue);
        Response.StatusCode = StatusCodes.Status200OK;
        return new JsonResult(dotNetObjList);
    }

    // GET: api/Patient/examinations/id
    [HttpGet("examinations/{id}")]
    public async Task<List<Examination>> GetPatientsExaminations(int id)
    {
        var examinations = _database.GetCollection<Examination>("MedicalExaminations");
        List<Examination> patientsExaminations = examinations.Find(e => e.PatinetId == id).ToList();
        return patientsExaminations;
    }

    // GET: api/Patient/examinations/id
    [HttpGet("drugs/{drug}/{id}")]
    public async Task<Prescription> GetPrescription(string drug, int id)
    {
        var patients = _database.GetCollection<Patient>("Patients");
        Patient patient = patients.Find(e => e.Id == id).FirstOrDefault();
        Prescription prescription = patient.MedicalRecord.Prescriptions.Find(e => e.DrugName == drug);
        return prescription;
    }

    // GET: api/Patient/examinations/id
    [HttpGet("drugs/{id}")]
    public async Task<List<MedicalInstruction>> GetMedicalInstructions(int id)
    {
        var patients = _database.GetCollection<Patient>("Patients");
        Patient patient = patients.Find(e => e.Id == id).FirstOrDefault();
        List<MedicalInstruction> medicalInstructions = new List<MedicalInstruction>();

        foreach (MedicalInstruction instruction in patient.MedicalRecord.MedicalInstructions)
        {
            if (checkDate(instruction))
            {
                medicalInstructions.Add(instruction);
            }
        }

        return medicalInstructions;
    }

    public bool checkDate(MedicalInstruction instruction)
    {
        var startDate = DateTime.Parse(instruction.StartDate);
        var endDate = DateTime.Parse(instruction.EndtDate);
        DateTime today = DateTime.Today;

        if (startDate <= today && today <= endDate)
        {
            return true;
        }

        return false;
    }

    // GET: api/Patient/examination/id
    [HttpGet("examination/{id}")]
    public async Task<Examination> GetExamination(int id)
    {
        var examinations = _database.GetCollection<Examination>("MedicalExaminations");
        Examination patientsExaminations = examinations.Find(e => e.Id == id).FirstOrDefault();
        return patientsExaminations;
    }

    // POST: api/Patient/examinationFilter
    [HttpPost("examinationFilter")]
    public async Task<List<Examination>> GetAvailableExamination([FromBody] ExaminationFilter filter)
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

        //There is no examination that fulfill s both time interval and doctor
        if (result.Count == 0)
        {
            if (filter.priority == "doctor")
            {
            }
            else if (filter.priority == "time")
            {
            }
        }

        return result;
    }

    // POST action
    [HttpPost("examinations")]
    public async Task<IActionResult> CreateExamination(Examination examination)
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
            return BadRequest();
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
        return Ok();
    }

    [HttpPost("examinationRequests")]
    public async Task<IActionResult> CreateRequest(ExaminationRequest request)
    {
        var requests = _database.GetCollection<ExaminationRequest>("ExaminationRequests");
        requests.InsertOne(request);
        return Ok();
    }

    [HttpPost("notifications")]
    public async Task<IActionResult> CreateNotification(DrugNotification notification)
    {
        if (notification.Time == "")
        {
            return BadRequest();
        }
        var notifications = _database.GetCollection<DrugNotification>("Notifications");
        notifications.InsertOne(notification);

        return Ok();
    }

    // PUT action
    [HttpPut("examinations/{id}")]
    public async Task<IActionResult> UpdateExamination(string id, [FromBody] Examination examination)
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
            return BadRequest();
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
            return Ok();
        }
        examinations.FindOneAndReplace(e => e.Id == int.Parse(id), examination);
        UpdateExaminationHistory("updated", patients, patient.Id);

        return Ok();

    }

    // DELETE action
    [HttpDelete("examinations/{id}")]
    public async Task<IActionResult> DeleteExamination(string id)
    {
        var examinations = _database.GetCollection<Examination>("MedicalExaminations");
        Examination examination = examinations.Find(item => item.Id == int.Parse(id)).FirstOrDefault();
        var patients = _database.GetCollection<Patient>("Patients");
        Patient patient = patients.Find(p => p.Id == examination.PatinetId).FirstOrDefault();

        var isTroll = TrollCheck(patient, "deleted/updated", 5);
        if (!isTroll)
        {
            return BadRequest();
        }

        if (isForRequest(examination))
        {
            ExaminationRequest request = new ExaminationRequest();
            request.Examination = examination;
            request.Status = 0;
            await CreateRequest(request);
            return Ok();
        }

        examinations.DeleteOne(item => item.Id == int.Parse(id));
        UpdateExaminationHistory("deleted", patients, patient.Id);

        return Ok();
    }

    public Boolean TrollCheck(Patient patient, String type, int n)
    {
        var patients = _database.GetCollection<Patient>("Patients");
        DateTime checkDate = DateTime.Today.AddDays(-30);
        int counter = 0;
        foreach (var entry in patient.ExaminationHistory)
        {
            DateTime entryDate = DateTime.Parse(entry.Date);
            if (entryDate > checkDate && type.Contains(entry.Type))
            {
                counter++;
            }
            if (entryDate < checkDate)
            {
                var update = Builders<Patient>.Update.PopFirst("examinationHistory");
                patients.UpdateOne(p => p.Id == patient.Id, update);
            }
        }

        if (counter > n)
        {
            var block = Builders<Patient>.Update.Set("active", "2");
            patients.UpdateOne(p => p.Id == patient.Id, block);
            return false;
        }
        return true;
    }
    public bool isForRequest(Examination oldExaminationData)
    {
        DateTime dt = DateTime.Today;
        DateTime dtOfExamination = DateTime.Parse(oldExaminationData.DateAndTimeOfExamination);
        return dt.AddDays(2) >= dtOfExamination;
    }
    public void UpdateExaminationHistory(string type, IMongoCollection<Patient> patients, int id)
    {
        ExaminationHistoryEntry newEntry = new ExaminationHistoryEntry();
        newEntry.Date = DateTime.Today.ToString("yyyy-MM-ddTHH:mm");
        newEntry.Type = type;
        var update = Builders<Patient>.Update.Push("examinationHistory", newEntry);
        patients.UpdateOne(p => p.Id == id, update);
    }
    public bool checkAvailability(List<Examination> examinations, DateTime examinationBegin, DateTime examinationEnd)
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
    public bool checkAdvancedAvilability(List<Examination> examinations, DateTime examinationTime)
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