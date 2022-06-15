using System.Net.Mail;
using System.Net;
public class ScheduleService : IScheduleService
{
    private IScheduleRepository _scheduleRepository;
    private IValidateExaminationService _validateExaminationService;
    private IRoomService _roomService;
    private IPatientService _patientService;
    private IUserRepository _userRepository;
    private IReferralService _referralService;

    public ScheduleService()
    {
        _roomService = new RoomService();
        _patientService = new PatientService();
        _scheduleRepository = new ScheduleRepository();
        _validateExaminationService = new ValidateExaminationService();
        _userRepository = new UserRepository();
        _referralService = new ReferralService();
    }

    public async Task<List<Examination>> GetAllExaminations()
    {
        return await _scheduleRepository.GetAllExaminations();
    }

    public async Task<List<Examination>> GetAllDoctorsExaminations(int doctorId)
    {
        return await _scheduleRepository.GetAllDoctorsExaminations(doctorId);
    }

    public async Task<List<Examination>> GetDoctorsExaminationsSchedule(int doctorId, string dateOfSearch)
    {
        var doctorsExaminations = await _scheduleRepository.GetAllDoctorsExaminations(doctorId);

        List<Examination> doctorsExaminationsInDateRange = new List<Examination>();

        //range of days for doctors schedule
        DateTime firstDay = DateTime.Parse(dateOfSearch);
        string threeDaysAfter = firstDay.AddDays(3).ToString();

        foreach (Examination examination in doctorsExaminations)
        {
            var answer = _validateExaminationService.IsDateInBetween(dateOfSearch, threeDaysAfter, examination.DateAndTimeOfExamination);
            if (answer)
            {
                doctorsExaminationsInDateRange.Add(examination);
            }
        }

        return doctorsExaminationsInDateRange;
    }

    public async Task<List<Examination>> GetAllPatientsExaminations(int patientId)
    {
        return await _scheduleRepository.GetAllPatientsExaminations(patientId);
    }

    public async Task SaveExamination(Examination examination)
    {
        await _scheduleRepository.InsertExamination(examination);
    }

    public async Task<Examination> GetExamination(int id)
    {
        return await _scheduleRepository.GetExamination(id);
    }

    public async Task UpdateExamination(int id, Examination examination)
    {
        await _scheduleRepository.UpdateExamination(id, examination);
    }

    public async Task DeleteExamination(int id)
    {
        await _scheduleRepository.DeleteExamination(id);
    }

    public async Task<bool> ExaminationScheduledAtThatTime(Renovation renovation)
    {
        List<Examination> examinations = await _scheduleRepository.GetAllExaminationsInRoom(renovation.Room);

        if (renovation.Room2 != null)
        {
            List<Examination> examinations2 = await _scheduleRepository.GetAllExaminationsInRoom(renovation.Room2);
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


    public bool isDurationValid(Examination newExamination)
    {
        if (newExamination.DurationOfExamination <= 15 || newExamination.DurationOfExamination >= 200)
        {
            return false;
        }
        return true;
    }

    public async Task<bool> doesSpecializedDoctorExist(int doctorId, string specialization)
    {
        if (specialization == "none")
        {
            return true;
        }
        List<Employee> specializedDoctors = await _userRepository.GetSpecializedDoctors(specialization);
        if (specializedDoctors.Count() == 0)
        {
            return false;
        }
        return true;
    }

    public async Task CreateRefferedExamination(Examination newExamination, string specialization, int referralid)
    {

        if (specialization != "none")
        {
            List<Employee> specializedDoctors = await _userRepository.GetSpecializedDoctors(specialization);
            Random rnd = new Random();
            newExamination.DoctorId = specializedDoctors[rnd.Next(0, specializedDoctors.Count() - 1)].Id;
        }


        var examinations = _scheduleRepository.GetAllExaminations();

        var newExaminationDate = DateTime.Now.AddDays(1);


        DateTime upperDateTimelimit;
        DateTime lowerDateTimelimit;

        while (true)
        {
            newExamination.DateAndTimeOfExamination = newExaminationDate.ToString("yyyy-MM-ddTHH:mm");
            if (await _validateExaminationService.IsExaminationValid(newExamination))
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
        await _scheduleRepository.InsertExamination(newExamination);

        await _referralService.DeletePatientReferral(referralid, newExamination);

    }

    public void SendTermNotificationEmailToPatient(Patient patient, Employee employee, string oldDateAndTime, string newDateAndTime, int? examId)
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

    public void SendTermNotificationEmailToDoctor(Patient patient, Employee employee, string oldDateAndTime, string newDateAndTime, int? examId)
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

        if (await _validateExaminationService.IsValidPatient(newExamination.PatinetId))
        {
            return new List<Examination>();
        }

        var room = await _roomService.GetRoomByType(roomType);

        newExamination.RoomName = room.Name;

        List<Employee> specializedDoctors = new List<Employee>();
        specializedDoctors = await _userRepository.GetSpecializedDoctors(specialization);

        var urgentExaminationDate = DateTime.Now;
        var urgentExaminationEnd = DateTime.Now.AddHours(2);

        var patient = _patientService.GetPatientById(newExamination.PatinetId);


        while (urgentExaminationDate <= urgentExaminationEnd)
        {
            newExamination.DateAndTimeOfExamination = urgentExaminationDate.ToString("yyyy-MM-ddTHH:mm");
            foreach (Employee doctor in specializedDoctors)
            {
                newExamination.DoctorId = doctor.Id;

                if (await _validateExaminationService.IsExaminationValid(newExamination))
                {
                    await _scheduleRepository.InsertExamination(newExamination);
                    return new List<Examination>();
                }
            }
            urgentExaminationDate = urgentExaminationDate.AddMinutes(10);
        }

        var examinationsAfterNow = await _scheduleRepository.GetExaminationsAfterNow(newExamination);

        List<Examination> fiveSortedExaminations = new List<Examination>();

        fiveSortedExaminations = examinationsAfterNow.Take(5).ToList();

        return fiveSortedExaminations;

    }

    public async Task CreateUrgentExaminationWithTermMoving(Examination newExamination)
    {

        var examinations = await _scheduleRepository.GetAllExaminations();

        List<Examination> reservedTimeFrames = new List<Examination>();

        foreach (Examination e in examinations)
        {
            if (e.RoomName == newExamination.RoomName && e.DoctorId == newExamination.DoctorId)
            {
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


        await _scheduleRepository.InsertExamination(newExamination);

        var iterationDateTime = DateTime.Now;

        foreach (Examination toMoveExamination in toMoveExaminations)
        {
            var oldDateAndTime = toMoveExamination.DateAndTimeOfExamination;
            while (true)
            {
                toMoveExamination.DateAndTimeOfExamination = iterationDateTime.ToString("yyyy-MM-ddTHH:mm");
                if (await _validateExaminationService.IsExaminationValid(toMoveExamination))
                {
                    Patient patient = await _patientService.GetPatientById(toMoveExamination.PatinetId);
                    Employee employee = await _userRepository.GetDoctor(toMoveExamination.DoctorId);

                    SendTermNotificationEmailToPatient(patient, employee, oldDateAndTime, toMoveExamination.DateAndTimeOfExamination, toMoveExamination.Id);
                    SendTermNotificationEmailToDoctor(patient, employee, oldDateAndTime, toMoveExamination.DateAndTimeOfExamination, toMoveExamination.Id);

                    await _scheduleRepository.UpdateExamination((int)toMoveExamination.Id, toMoveExamination);
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