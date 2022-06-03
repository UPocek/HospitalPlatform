public class ExaminationService : IExaminationService
{
    private IExaminationRepository examinationRepository;
    private IRoomRepository roomRepository;
    private IPatientRepository patientRepository;
    private IRenovationRepository renovationRepository;

    public ExaminationService()
    {
        examinationRepository = new ExaminationRepository();
        roomRepository = new RoomRepository();
        patientRepository = new PatientRepository();
        renovationRepository = new RenovationRepository();
    }

    public async Task<List<Examination>> GetAllExaminations()
    {
        return await examinationRepository.GetAllExaminations();
    }

    public async Task<List<Examination>> GetAllDoctorsExaminations(int doctorId)
    {
        return await examinationRepository.GetAllDoctorsExaminations(doctorId);
    }

    public async Task<List<Examination>> GetDoctorsExaminationsSchedule(int doctorId, string dateOfSearch)
    {
        var doctorsExaminations = await examinationRepository.GetAllDoctorsExaminations(doctorId);
        List<Examination> doctorsExaminationsInDateRange = new List<Examination>(); 
        DateTime firstDay = DateTime.Parse(dateOfSearch);
        string threeDaysAfter = firstDay.AddDays(3).ToString();
        foreach (Examination examination in doctorsExaminations)
        {
            var answer = IsDateInBetween(dateOfSearch, threeDaysAfter, examination.DateAndTimeOfExamination);
            if (answer)
            {
                doctorsExaminationsInDateRange.Add(examination);
            }
        }

        return doctorsExaminationsInDateRange;
    }

    public async Task<List<Examination>> GetAllPatientsExaminations(int patientId)
    {
        return await examinationRepository.GetAllPatientsExaminations(patientId);
    }

    public async Task SaveExamination(Examination examination)
    {
        await examinationRepository.InsertExamination(examination);
    }

    public async Task UpdateExamination(int id, Examination examination)
    {
        await examinationRepository.UpdateExamination(id, examination);
    }

    public async Task DeleteExamination(int id)
    {
        await examinationRepository.DeleteExamination(id);
    }

    public async Task<bool> IsRoomOccupied(string examinationRoomName, string dateAndTimeOfExamination, int durationOfExamination)
    {
        var possiblyOccupiedRooms = await examinationRepository.GetAllExaminations();

        foreach (Examination item in possiblyOccupiedRooms)
        {
            if (item.RoomName == examinationRoomName)
            {
                var answer = IsTimeInBetween(dateAndTimeOfExamination, durationOfExamination, item);
                if (answer) return true;
            }
        }

        return false;
    }

    public async Task<bool> IsRoomOccupiedForUpdate(string examinationRoomName, string dateAndTimeOfExamination, int durationOfExamination, int examinationId)
    {
        var possiblyOccupiedRooms = await examinationRepository.GetAllExaminations();

        foreach (Examination item in possiblyOccupiedRooms)
        {
            if (item.RoomName == examinationRoomName && item.Id != examinationId)
            {
                var answer = IsTimeInBetween(dateAndTimeOfExamination, durationOfExamination, item);
                if (answer) return true;
            }
        }

        return false;
    }

    public async Task<bool> IsRoomValid(string roomName)
    {
        var rooms = await roomRepository.GetAllRooms();

        foreach (Room room in rooms)
        {
            if (room.Name == roomName && room.InRenovation == false)
            {
                return true;
            }
        }

        return false;
    }

    public async Task<bool> IsValidPatient(int id)
    {
        var resultingPatient = await patientRepository.GetPatientById(id);

        if (resultingPatient == null)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> IsRoomInRenovation(string roomName, string examinationDate)
    {
        var renovations = await renovationRepository.GetAllRenovations();

        foreach (Renovation r in renovations)
        {
            var answer = IsDateInBetween(r.StartDate, r.EndDate, examinationDate);
            if (answer) return true;
        }

        return false;
    }

    public async Task<bool> IsPatientFree(int id, string dateAndTimeOfExamination, int durationOfExamination)
    {
        var patientsExaminations = await examinationRepository.GetAllPatientsExaminations(id);

        foreach (Examination examination in patientsExaminations)
        {
            var answer = IsTimeInBetween(dateAndTimeOfExamination, durationOfExamination, examination);
            if (answer) return false;
        }

        return true;
    }

    public async Task<bool> IsDoctorFree(int id, string dateAndTimeOfExamination, int durationOfExamination)
    {
        var doctorsExaminations = await examinationRepository.GetAllDoctorsExaminations(id);

        foreach (Examination examination in doctorsExaminations)
        {
            var answer = IsTimeInBetween(dateAndTimeOfExamination, durationOfExamination, examination);
            if (answer) return false;
        }

        return true;
    }

    public bool IsTimeInBetween(string dateAndTimeOfExamination, int durationOfExamination, Examination examination)
    {
        DateTime itemBegin = DateTime.Parse(examination.DateAndTimeOfExamination);
        DateTime itemEnd = itemBegin.AddMinutes(examination.DurationOfExamination);
        DateTime examinationBegin = DateTime.Parse(dateAndTimeOfExamination);
        DateTime examinationEnd = examinationBegin.AddMinutes(durationOfExamination);
        if ((examinationBegin >= itemBegin && examinationBegin <= itemEnd) || (examinationEnd >= itemBegin && examinationEnd <= itemEnd))
        {
            return true;
        }

        return false;
    }

    public bool IsDateInBetween(string startDate, string endDate, string examinationDate)
    {
        DateTime begin = DateTime.Parse(startDate);
        DateTime end = DateTime.Parse(endDate);
        DateTime examinationDateParsed = DateTime.Parse(examinationDate);

        if (begin <= examinationDateParsed && end >= examinationDateParsed)
        {
            return true;
        }

        return false;
    }

    public async Task<bool> IsExaminationValid(Examination examination)
    {
        var isValidPatient = await IsValidPatient(examination.PatinetId);
        var isValidRoom = await IsRoomValid(examination.RoomName);
        var isRoomInRenovation = await IsRoomInRenovation(examination.RoomName, examination.DateAndTimeOfExamination);
        var isPatientFree = await IsPatientFree(examination.PatinetId, examination.DateAndTimeOfExamination, examination.DurationOfExamination);
        var isDoctorFree = await IsDoctorFree(examination.DoctorId, examination.DateAndTimeOfExamination, examination.DurationOfExamination);

        return (isValidRoom && isValidPatient && !isRoomInRenovation && isPatientFree && isDoctorFree);
    }

    public async Task<bool> IsNewExaminationValid(Examination examination)
    {
        var isExaminationValid = await IsExaminationValid(examination);
        var isOccupiedRoom = await IsRoomOccupied(examination.RoomName, examination.DateAndTimeOfExamination, examination.DurationOfExamination);

        return (isExaminationValid && !isOccupiedRoom);
    }

    public async Task<bool> IsUpdatedExaminationValid(Examination examination)
    {
        var isOccupiedRoom = await IsRoomOccupiedForUpdate(examination.RoomName, examination.DateAndTimeOfExamination, examination.DurationOfExamination, (int)examination.Id);
        var isExaminationValid = await IsExaminationValid(examination);

        return (isExaminationValid && !isOccupiedRoom);
    }

    public async Task<bool> ExaminationScheduledAtThatTime(Renovation renovation)
    {
        List<Examination> examinations = await examinationRepository.GetAllExaminationsInRoom(renovation.Room);

        if (renovation.Room2 != null)
        {
            List<Examination> examinations2 = await examinationRepository.GetAllExaminationsInRoom(renovation.Room2);
            examinations = examinations.Concat(examinations2).ToList();
        }

        foreach (var examination in examinations)
        {
            bool examinationOverlapsRenovation = DateTime.Parse(examination.DateAndTimeOfExamination) >= DateTime.Parse(renovation.StartDate) && DateTime.Parse(examination.DateAndTimeOfExamination) <= DateTime.Parse(renovation.EndDate);
            if (examinationOverlapsRenovation)
            {
                return true;
            }
        }

        return false;
    }

}