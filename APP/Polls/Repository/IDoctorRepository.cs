public interface IDoctorRepository
{
    public Task<List<PollForDoctors>> GetAllDoctors();
}