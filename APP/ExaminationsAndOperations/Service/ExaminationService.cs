using System.Net.Mail;
using System.Net;
public class ExaminationService : IExaminationService
{
    private IExaminationRepository examinationRepository;
    private IRoomRepository roomRepository;
    private IPatientRepository patientRepository;
    private IRenovationRepository renovationRepository;
    private IUserRepository userRepository;
    private IReferralRepository referralRepository;

    public ExaminationService()
    {
        examinationRepository = new ExaminationRepository();
        roomRepository = new RoomRepository();
        patientRepository = new PatientRepository();
        renovationRepository = new RenovationRepository();
        userRepository = new UserRepository();
        referralRepository = new ReferralRepository();
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

    public async Task<List<Examination>> GetAvailableExamination(ExaminationFilter filter){
        return await examinationRepository.GetAvailableExamination(filter);
    }


    public async Task SaveExamination(Examination examination)
    {
        await examinationRepository.InsertExamination(examination);
    }

    public async Task CreateExamination(Examination examination)
    {
        await examinationRepository.CreateExamination(examination);
    }

    public async Task<Examination> GetExamination(int id){
        return await examinationRepository.GetExamination(id);
    }

    public async Task UpdateExamination(int id, Examination examination)
    {
        await examinationRepository.UpdateExamination(id, examination);
    }
    public async Task UpdatePatientsExamination(string id, Examination examination)
    {
        await examinationRepository.UpdatePatientsExamination(id, examination);
    }
    public async Task DeleteExamination(int id)
    {
        await examinationRepository.DeleteExamination(id);
    }

    public async Task DeletePatientsExamination(int id)
    {
        await examinationRepository.DeletePatientsExamination(id);
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

    public async Task<bool> IsPatientFree(int id, string dateAndTimeOfExamination, int durationOfExamination, int examinationId)
    {
        var patientsExaminations = await examinationRepository.GetAllPatientsExaminations(id);

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
        var doctorsExaminations = await examinationRepository.GetAllDoctorsExaminations(id);

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



    public bool isDurationValid(Examination newExamination){
        if (newExamination.DurationOfExamination <= 15 || newExamination.DurationOfExamination >= 200)
        {
            return false;
        }
        return true;
    }

    public async Task<bool> doesSpecializedDoctorExist(int doctorId,string specialization){
        if(specialization == "none"){
            return true;
        }
        List<Employee> specializedDoctors = await userRepository.GetSpecializedDoctors(specialization);
        if(specializedDoctors.Count() == 0){
            return false;
        }
        return true;
    }

    public async Task<bool> doesRoomExist(string roomName){
        Room room = await roomRepository.GetRoomByName(roomName);
        if (room == null){
            return false;
        }
        return true;
    }

    public async Task CreateRefferedExamination(Examination newExamination, string specialization, int referralid)
    {

        if (specialization != "none")
        {
            List<Employee> specializedDoctors = await userRepository.GetSpecializedDoctors(specialization);
            Random rnd = new Random();
            newExamination.DoctorId = specializedDoctors[rnd.Next(0, specializedDoctors.Count() - 1)].Id;
        }


        var examinations = examinationRepository.GetAllExaminations();

        var newExaminationDate = DateTime.Now.AddDays(1);


        DateTime upperDateTimelimit;
        DateTime lowerDateTimelimit;

        while (true)
        {
            newExamination.DateAndTimeOfExamination = newExaminationDate.ToString("yyyy-MM-ddTHH:mm");
            if (await IsExaminationValid(newExamination))
            {
                lowerDateTimelimit = new DateTime(newExaminationDate.Year, newExaminationDate.Month, newExaminationDate.Day, 8, 0, 0);
                upperDateTimelimit = new DateTime(newExaminationDate.Year, newExaminationDate.Month, newExaminationDate.Day, 23, 59, 0);
                if (newExaminationDate >= lowerDateTimelimit && newExaminationDate <= upperDateTimelimit)
                {
                    break;
                }
                else
                {
                    newExaminationDate = newExaminationDate.AddMinutes(30);
                    continue;
                }
            }

            else
            {
                newExaminationDate = newExaminationDate.AddMinutes(30);
            }

        }
        await examinationRepository.InsertExamination(newExamination);

        referralRepository.DeletePatientReferral(referralid, newExamination);

    }

    public void SendTermNotificationEmailToPatient(Patient patient,Employee employee, string oldDateAndTime, string newDateAndTime, int? examId)
    {
        var smptClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("teamnineMedical@gmail.com", "teamnine"),
            EnableSsl = true,
        };

        string messageDoctor = "Hello " + employee.FirstName + " " + employee.LastName
                    + "\n\n\nYour examination id:" + examId + " has been moved from " + oldDateAndTime + " to " +
                    newDateAndTime + ".\n\n\nPatient in question:\nid: " + patient.Id +
                    "\nName: " + patient.FirstName + "\nSurname: " + patient.LastName + "\n Have a nice day!";


        var mailMessageDoctor = new MailMessage
        {
            From = new MailAddress(employee.Email),
            Subject = "TeamNine Medical Team - IMPORTANT - examination moved",
            Body = messageDoctor,
            IsBodyHtml = true,
        };

        mailMessageDoctor.To.Add("teamnineMedical@gmail.com");
        smptClient.Send(mailMessageDoctor);
    }

    public void SendTermNotificationEmailToDoctor(Patient patient,Employee employee, string oldDateAndTime, string newDateAndTime, int? examId)
    {
        var smptClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("teamnineMedical@gmail.com", "teamnine"),
            EnableSsl = true,
        };

        string messagePatient = "Hello " + patient.FirstName + " " + patient.LastName
                    + "\n\n\nYour examination id:" + examId + " has been moved from " + oldDateAndTime + " to " +
                    newDateAndTime + ".\n\n\nDoctor in question:" +
                    "\nName: " + employee.FirstName + "\nSurname: " + employee.LastName + "\n Have a nice day!";


        var mailMessagePatient = new MailMessage
        {
            From = new MailAddress(employee.Email),
            Subject = "TeamNine Medical Team - IMPORTANT - examination moved",
            Body = messagePatient,
            IsBodyHtml = true,
        };

        mailMessagePatient.To.Add("teamnineMedical@gmail.com");
        smptClient.Send(mailMessagePatient);
    }


    public async Task<List<Examination>> CreateUrgentExamination(Examination newExamination, string specialization)
    {

        string roomType;
        if (newExamination.TypeOfExamination == "visit")
        {
            roomType = "examination room";
        }
        else
        {
            roomType = "operation room";
        }

        if (await patientRepository.GetPatientById(newExamination.PatinetId) == null)
        {
            return new List<Examination>();
        }

        var room = await roomRepository.GetRoomByType(roomType);

        newExamination.RoomName = room.Name;

        List<Employee> specializedDoctors = new List<Employee>();
        specializedDoctors = await userRepository.GetSpecializedDoctors(specialization);

        var urgentExaminationDate = DateTime.Now;
        var urgentExaminationEnd = DateTime.Now.AddHours(2);

        var patient = patientRepository.GetPatientById(newExamination.PatinetId);


        while (urgentExaminationDate <= urgentExaminationEnd)
        {
            newExamination.DateAndTimeOfExamination = urgentExaminationDate.ToString("yyyy-MM-ddTHH:mm");
            foreach (Employee doctor in specializedDoctors)
            {
                newExamination.DoctorId = doctor.Id;

                if (await IsExaminationValid(newExamination))
                {
                    await examinationRepository.InsertExamination(newExamination);
                    return new List<Examination>();
                }
            }
            urgentExaminationDate = urgentExaminationDate.AddMinutes(10);
        }

        var examinationsAfterNow = await examinationRepository.GetExaminationsAfterNow(newExamination);

        List<Examination> fiveSortedExaminations = new List<Examination>();

        fiveSortedExaminations = examinationsAfterNow.Take(5).ToList();

        return fiveSortedExaminations;

    }

    public async Task CreateUrgentExaminationWithTermMoving(Examination newExamination)
    {

        var examinations = await examinationRepository.GetAllExaminations();

        List<Examination> reservedTimeFrames = new List<Examination>();

        foreach(Examination e in examinations){
            if(e.RoomName == newExamination.RoomName && e.DoctorId == newExamination.DoctorId){
                reservedTimeFrames.Add(e);
            }
        }

        List<Examination> toMoveExaminations = new List<Examination>();

        var newExaminationBegin = DateTime.Parse(newExamination.DateAndTimeOfExamination);
        var newExaminationEnd = newExaminationBegin.AddMinutes(newExamination.DurationOfExamination);

        DateTime toMoveExamBegin;

        foreach (Examination e in reservedTimeFrames)
        {
            toMoveExamBegin = DateTime.Parse(e.DateAndTimeOfExamination);
            if (newExaminationBegin <= toMoveExamBegin && newExaminationEnd >= toMoveExamBegin)
            {
                toMoveExaminations.Add(e);
            }
        }


        await examinationRepository.InsertExamination(newExamination);

        var iterationDateTime = DateTime.Now;

        foreach (Examination toMoveExamination in toMoveExaminations)
        {
            var oldDateAndTime = toMoveExamination.DateAndTimeOfExamination;
            while (true)
            {
                toMoveExamination.DateAndTimeOfExamination = iterationDateTime.ToString("yyyy-MM-ddTHH:mm");
                if (await IsExaminationValid(toMoveExamination))
                {
                    Patient patient = await patientRepository.GetPatientById(toMoveExamination.PatinetId);
                    Employee employee = await userRepository.GetDoctor(toMoveExamination.DoctorId);

                    SendTermNotificationEmailToPatient(patient, employee, oldDateAndTime, toMoveExamination.DateAndTimeOfExamination, toMoveExamination.Id);
                    SendTermNotificationEmailToDoctor(patient, employee, oldDateAndTime, toMoveExamination.DateAndTimeOfExamination, toMoveExamination.Id);

                    await examinationRepository.UpdateExamination((int)toMoveExamination.Id,toMoveExamination);
                    break;
                }


                else
                {
                    iterationDateTime = iterationDateTime.AddMinutes(5);
                }
            }
        }
    }

}