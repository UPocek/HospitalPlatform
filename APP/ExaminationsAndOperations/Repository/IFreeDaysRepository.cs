public interface IFreeDaysRepository
{
    public Task removeStaleFreeDaysRequests();
    public Task<List<FreeDayRequest>> GetAllFreeDaysRequests();
    public Task<List<FreeDayRequest>> GetAllDoctorsFreeDaysRequests(int doctorId);
    public Task RequestFreeDays(FreeDayRequest freeDayRequest);
}