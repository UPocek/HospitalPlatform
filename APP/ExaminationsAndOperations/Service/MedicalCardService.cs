public class MedicalCardService : IMedicalCardService
{
    private IMedicalCardRepository _medicalCardRepository;

    public MedicalCardService()
    {
        _medicalCardRepository = new MedicalCardRepository();
    }

    public async Task<MedicalCard> GetMedicalCard(int patientId)
    {
        return await _medicalCardRepository.GetMedicalCard(patientId);
    }

}