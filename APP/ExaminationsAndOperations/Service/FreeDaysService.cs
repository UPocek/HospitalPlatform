using System.Net.Mail;
using System.Net;
public class FreeDaysService : IFreeDaysService
{
    private IFreeDaysRepository _freeDaysRepository;
    private IValidateExaminationService _validateExaminationService;
    private IScheduleService _scheduleService;
    private IUserRepository _userRepository;
    public FreeDaysService()
    {
        _freeDaysRepository = new FreeDaysRepository();
        _scheduleService = new ScheduleService();
        _validateExaminationService = new ValidateExaminationService();
        _userRepository = new UserRepository();
    }


    public async Task<List<FreeDayRequest>> GetAllDoctorsFreeDaysRequests(int doctorId)
    {
        await _freeDaysRepository.DeleteStaleFreeDaysRequests();
        return await _freeDaysRepository.GetAllDoctorsFreeDaysRequests(doctorId);
    }

    public async Task<List<FreeDayRequest>> GetAllFreeDaysRequests()
    {
        await _freeDaysRepository.DeleteStaleFreeDaysRequests();
        return await _freeDaysRepository.GetAllFreeDaysRequests();
    }

    public async Task DeleteFreeDaysRequest(string id){
        await _freeDaysRepository.DeleteFreeDaysRequest(id);
    }
    
    public async Task AddFreeDay(int doctorId,int duration,FreeDay fd){
        Employee doctor = await _userRepository.GetDoctor(doctorId);

        DateTime startDate = DateTime.Parse(fd.From);
        DateTime endDate = startDate.AddDays(duration);
        string endString = endDate.ToString("yyyy-MM-dd");
        fd.To = endString;
        
        await _userRepository.UpdateDoctorFreeDays(doctorId,fd);
    }
    public void SendDeclineNotification(string mail,string why){
        var smptClient = new SmtpClient("smtp-mail.outlook.com",587)
        {
            Credentials = new NetworkCredential("teamNineMedical@outlook.com","teamnine123"),
            EnableSsl = true
        };

        string messageDoctor = "Hello your free days request has been declined, reason:\n" + why + "\n Have a nice day!";


        var mailMessageDoctor = new MailMessage
        {
            From = new MailAddress("teamNineMedical@outlook.com"),
            Subject = "TeamNine Medical Team - free days declined",
            Body = messageDoctor,
            IsBodyHtml = true,
        };

        mailMessageDoctor.To.Add("teamNineMedical@outlook.com");
        smptClient.Send(mailMessageDoctor);
    }

    public void SendAcceptNotification(string mail){
        var smptClient = new SmtpClient("smtp-mail.outlook.com",587)
        {
            Credentials = new NetworkCredential("teamNineMedical@outlook.com","teamnine123"),
            EnableSsl = true
        };

        string messageDoctor = "Hello your free days request has been accepted,Have a nice day!";


        var mailMessageDoctor = new MailMessage
        {
            From = new MailAddress("teamNineMedical@outlook.com"),
            Subject = "TeamNine Medical Team - free days declined",
            Body = messageDoctor,
            IsBodyHtml = true,
        };

        mailMessageDoctor.To.Add("teamNineMedical@outlook.com");
        smptClient.Send(mailMessageDoctor);
    }

    public async Task SaveRequest(FreeDayRequest freeDayRequest)
    {
        if (freeDayRequest.Urgent)
        {
            freeDayRequest = SetStatusOfRequest(freeDayRequest);
        }
        await _freeDaysRepository.RequestFreeDays(freeDayRequest);
    }

    public async Task<bool> SendDoctorsRequest(FreeDayRequest freeDayRequest)
    {
        freeDayRequest.RequestDay = DateTime.Now.ToString("yyyy-MM-dd");
        if (await IsRequestValid(freeDayRequest))
        {
            SetStatusOfRequest(freeDayRequest);
            await SaveRequest(freeDayRequest);
            return true;
        }
        return false;
    }

    public FreeDayRequest SetStatusOfRequest(FreeDayRequest freeDayRequest)
    {
        if (freeDayRequest.Urgent)
        {
            freeDayRequest.Status = "approved";
        }
        else
        {
            freeDayRequest.Status = "waiting";
        }

        return freeDayRequest;
    }


    public async Task<bool> IsRequestValid(FreeDayRequest freeDayRequest)
    {
        var hasMoreThenTwoDays = HasMoreThenTwoDays(freeDayRequest);
        var noUpcomingFreeDays = await NoUpcomingFreeDays(freeDayRequest);
        var noExaminations = await NoExaminationsInPeriod(freeDayRequest);
        var isValidUrgent = IsValidUrgent(freeDayRequest);

        return (noUpcomingFreeDays && noExaminations && isValidUrgent && hasMoreThenTwoDays);
    }

    public async Task<bool> NoUpcomingFreeDays(FreeDayRequest freeDayRequest)
    {
        var freeDays = await _freeDaysRepository.GetAllDoctorsFreeDaysRequests(freeDayRequest.DoctorId);
        DateTime requestBegin = DateTime.Parse(freeDayRequest.StartDay);
        DateTime requestEnd = requestBegin.AddDays(freeDayRequest.Duration);
        foreach (FreeDayRequest request in freeDays)
        {
            DateTime currentRequestBegin = DateTime.Parse(request.StartDay);
            DateTime currentRequestEnd = currentRequestBegin.AddDays(request.Duration);
            var isBeginDateInBetween = _validateExaminationService.IsDateInBetween(currentRequestBegin.ToString(), currentRequestEnd.ToString(), requestBegin.ToString());
            var isEndDateInBetween = _validateExaminationService.IsDateInBetween(currentRequestBegin.ToString(), currentRequestEnd.ToString(), requestEnd.ToString());
            if (isBeginDateInBetween || isEndDateInBetween)
            {
                return false;
            }
        }
        return true;
    }

    public async Task<bool> NoExaminationsInPeriod(FreeDayRequest freeDayRequest)
    {
        var doctorsExaminations = await _scheduleService.GetAllDoctorsExaminations(freeDayRequest.DoctorId);
        DateTime requestBegin = DateTime.Parse(freeDayRequest.StartDay);
        DateTime requestEnd = requestBegin.AddDays(freeDayRequest.Duration);
        foreach (Examination examination in doctorsExaminations)
        {
            if (_validateExaminationService.IsDateInBetween(requestBegin.ToString(), requestEnd.ToString(), examination.DateAndTimeOfExamination))
            {
                return false;
            }
        }
        return true;
    }

    public bool HasMoreThenTwoDays(FreeDayRequest freeDayRequest)
    {
        DateTime freeDaysBegin = DateTime.Parse(freeDayRequest.StartDay);
        DateTime requestDate = DateTime.Parse(freeDayRequest.RequestDay);
        if ((freeDaysBegin - requestDate).TotalDays >= 2)
        {
            return true;
        }
        return false;
    }

    public bool IsValidUrgent(FreeDayRequest freeDayRequest)
    {
        if (freeDayRequest.Duration > 5)
        {
            return false;
        }
        return true;
    }

}