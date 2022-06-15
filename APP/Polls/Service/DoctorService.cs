public class DoctorService : IDoctorService
{
    private IDoctorRepository _doctorRepository;

    public DoctorService()
    {
        _doctorRepository = new DoctorRepository();
    }

    public async Task<List<PollForDoctors>> GetAllDoctors()
    {
        return await _doctorRepository.GetAllDoctors();
    }
}