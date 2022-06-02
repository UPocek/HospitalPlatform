public class PollService : IPollService
{
    private IPollRepository repository;
    private IDoctorRepository repository2;

    public PollService()
    {
        repository = new PollRepository();
        repository2 = new DoctorRepository();
    }

    public async Task<Hospital> GetHospitalPolls()
    {
        return await repository.GetHospital();
    }

    public async Task<List<PollForDoctors>> GetDoctorPolls()
    {
        return await repository2.GetAllDoctors();
    }
}