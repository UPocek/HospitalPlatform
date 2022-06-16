using MongoDB.Driver;
public class CronFreeDayRequestService : CronJobService
{
    private IScheduleService _scheduleService;
    private IFreeDaysRepository _freeDaysRepository;
    private IValidateExaminationService _validateExaminationService;
    
    public CronFreeDayRequestService(IScheduleConfig<CronFreeDayRequestService> config)
        : base(config.CronExpression, config.TimeZoneInfo)
    {
        _scheduleService = new ScheduleService();
        _freeDaysRepository = new FreeDaysRepository();
        _validateExaminationService = new ValidateExaminationService();
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        return base.StartAsync(cancellationToken);
    }

    public override async Task DoWork(CancellationToken cancellationToken)
    {
        List<FreeDayRequest> freeDayRequests = await _freeDaysRepository.GetAllFreeDaysRequests();
        foreach (FreeDayRequest fdr in freeDayRequests){

            List<Examination> doctorsExaminations =  await _scheduleService.GetAllDoctorsExaminations(fdr.DoctorId);
            DateTime requestBegin = DateTime.Parse(fdr.StartDay);
            DateTime requestEnd = requestBegin.AddDays(fdr.Duration);

            foreach (Examination examination in doctorsExaminations)
            {
                if (_validateExaminationService.IsDateInBetween(requestBegin.ToString(), requestEnd.ToString(), examination.DateAndTimeOfExamination))
                {
                    await _freeDaysRepository.DeleteFreeDaysRequest(fdr._Id);
                }
            }
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return base.StopAsync(cancellationToken);
    }

}