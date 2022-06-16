public class ValidateExaminationService : IValidateExaminationService
{
    private IScheduleRepository _scheduleRepository;
    private IRoomService _roomService;
    private IPatientService _patientService;
    private IRenovationService _renovationService;
    private IUserRepository _userRepository;

    public ValidateExaminationService()
    {
        _scheduleRepository = new ScheduleRepository();
        _roomService = new RoomService();
        _patientService = new PatientService();
        _renovationService = new RenovationService();
        _userRepository = new UserRepository();
    }
    public async Task<bool> IsRoomOccupied(string examinationRoomName, string dateAndTimeOfExamination, int durationOfExamination)
    {
        var possiblyOccupiedRooms = await _scheduleRepository.GetAllExaminations();

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
        var possiblyOccupiedRooms = await _scheduleRepository.GetAllExaminations();

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
        var rooms = await _roomService.GetAllRooms();

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
        var resultingPatient = await _patientService.GetPatientById(id);

        if (resultingPatient == null)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> IsRoomInRenovation(string roomName, string examinationDate)
    {
        var renovations = await _renovationService.GetAllRenovations();

        foreach (Renovation r in renovations)
        {
            var answer = IsDateInBetween(r.StartDate, r.EndDate, examinationDate);
            if (answer) return true;
        }

        return false;
    }

    public async Task<bool> IsPatientFree(int id, string dateAndTimeOfExamination, int durationOfExamination, int examinationId)
    {
        var patientsExaminations = await _scheduleRepository.GetAllPatientsExaminations(id);

        foreach (Examination examination in patientsExaminations)
        {
            if (examinationId != examination.Id)
            {
                var answer = IsTimeInBetween(dateAndTimeOfExamination, durationOfExamination, examination);
                if (answer) return false;
            }
        }

        return true;
    }

    public async Task<bool> IsDoctorFree(int id, string dateAndTimeOfExamination, int durationOfExamination, int examinationId)
    {
        var doctorsExaminations = await _scheduleRepository.GetAllDoctorsExaminations(id);

        Employee doctor = await _userRepository.GetDoctor(id);

        foreach(FreeDay fd in doctor.FreeDays){
            if (fd.Status == "active"){
                return false;
            }
        }
        foreach (Examination examination in doctorsExaminations)
        {
            if (examinationId != examination.Id)
            {
                var answer = IsTimeInBetween(dateAndTimeOfExamination, durationOfExamination, examination);
                if (answer) return false;
            }
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
        var isPatientFree = await IsPatientFree(examination.PatinetId, examination.DateAndTimeOfExamination, examination.DurationOfExamination, (int)examination.Id);
        var isDoctorFree = await IsDoctorFree(examination.DoctorId, examination.DateAndTimeOfExamination, examination.DurationOfExamination, (int)examination.Id);

        return (isValidRoom && isValidPatient && !isRoomInRenovation && isPatientFree && isDoctorFree);
    }

    public async Task<bool> IsNewExaminationValid(Examination examination)
    {
        examination.Id = 0;
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
}