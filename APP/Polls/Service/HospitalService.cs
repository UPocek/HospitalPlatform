public class HospitalService : IHospitalService
{
    private IHospitalRepository _hospitalRepository;

    public HospitalService()
    {
        _hospitalRepository = new HospitalRepository();
    }

    public async Task<Hospital> GetHospital()
    {
        return await _hospitalRepository.GetHospital();
    }
}