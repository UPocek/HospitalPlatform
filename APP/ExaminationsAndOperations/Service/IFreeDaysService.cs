public interface IFreeDaysService
{
    public Task<List<FreeDayRequest>> GetAllDoctorsFreeDaysRequests(int doctorId);

}