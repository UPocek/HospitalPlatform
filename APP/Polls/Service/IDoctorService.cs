public interface IDoctorService
{
    public Task<List<PollForDoctors>> GetAllDoctors();
}