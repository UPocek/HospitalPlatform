public class PollService : IPollService
{
    private IHospitalService _hospitalService;
    private IDoctorService _doctorService;

    public PollService()
    {
        _hospitalService = new HospitalService();
        _doctorService = new DoctorService();
    }

    public async Task<Hospital> GetHospitalPolls()
    {
        return await _hospitalService.GetHospital();
    }

    public async Task<List<PollForDoctors>> GetDoctorPolls()
    {
        return await _doctorService.GetAllDoctors();
    }
}