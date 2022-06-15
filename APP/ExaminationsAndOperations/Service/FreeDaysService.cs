public class FreeDaysService : IFreeDaysService
{
    private IFreeDaysRepository _freeDaysRepository;
    private IScheduleService _scheduleService;
    private IValidateExaminationService _validateExaminationService;

    public FreeDaysService()
    {
        _freeDaysRepository = new FreeDaysRepository();
        _scheduleService = new ScheduleService();
        _validateExaminationService = new ValidateExaminationService();
    }

    public async Task<List<FreeDayRequest>> GetAllDoctorsFreeDaysRequests(int doctorId)
    {
        return await _freeDaysRepository.GetAllDoctorsFreeDaysRequests(doctorId);
    }
}