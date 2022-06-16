using MongoDB.Driver;
public class CronFreeDayStatusService : CronJobService
{
    private IUserRepository _userRepository;
    public CronFreeDayStatusService(IScheduleConfig<CronFreeDayStatusService> config)
        : base(config.CronExpression, config.TimeZoneInfo)
    {
        _userRepository = new UserRepository();
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        return base.StartAsync(cancellationToken);
    }

    public override async Task DoWork(CancellationToken cancellationToken)
    {
        List<Employee> doctors = await _userRepository.GetDoctors();
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        foreach (Employee doctor in doctors){
            foreach(FreeDay fd in doctor.FreeDays){
                if(String.Compare(fd.To,today) >= 0 && String.Compare(fd.From,today) <= 0){
                    fd.Status = "active";
                }
                else if(String.Compare(today,fd.To) <= 0 && String.Compare(today,fd.From) <= 0){
                    fd.Status = "upcoming";
                }
                else{
                    fd.Status = "finished";
                }
                await _userRepository.UpdateDoctorFreeDays(doctor.Id,doctor);
            }

        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return base.StopAsync(cancellationToken);
    }

}