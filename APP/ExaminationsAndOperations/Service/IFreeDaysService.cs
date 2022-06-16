public interface IFreeDaysService
{
    public Task<List<FreeDayRequest>> GetAllFreeDaysRequests();
    public Task<List<FreeDayRequest>> GetAllDoctorsFreeDaysRequests(int doctorId);
    public Task SaveRequest(FreeDayRequest freeDayRequest);
    public Task<bool> SendDoctorsRequest(FreeDayRequest freeDayRequest);
    public Task<bool> IsRequestValid(FreeDayRequest freeDayRequest);
    public Task<bool> NoUpcomingFreeDays(FreeDayRequest freeDayRequest);
    public bool HasMoreThenTwoDays(FreeDayRequest freeDayRequest);
    public Task<bool> NoExaminationsInPeriod(FreeDayRequest freeDayRequest);
    public bool IsValidUrgent(FreeDayRequest freeDayRequest);

}