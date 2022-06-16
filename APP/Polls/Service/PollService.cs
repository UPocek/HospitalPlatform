public class PollService : IPollService
{
    private IHospitalService _hospitalService;
    private IDoctorService _doctorService;

    private IPollRepository _pollRepository;


   
    public PollService()
    {
        _hospitalService = new HospitalService();
        _doctorService = new DoctorService();
        _pollRepository = new PollRepository();
    }

    public async Task<Hospital> GetHospitalPolls()
    {
        return await _hospitalService.GetHospital();
    }

    public async Task<List<PollForDoctors>> GetDoctorPolls()
    {
        return await _doctorService.GetAllDoctors();
    }

    public async Task PostPoll(Poll poll)
    {
        await _pollRepository.PostPoll(poll);
    }
}