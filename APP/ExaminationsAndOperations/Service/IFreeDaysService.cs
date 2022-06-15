public interface IFreeDaysService
{
    public Task<List<FreeDayRequest>> GetAllFreeDaysRequests();
    public Task<List<FreeDayRequest>> GetAllDoctorsFreeDaysRequests(int doctorId);

}