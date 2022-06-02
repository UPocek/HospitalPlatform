public class MedicalCardService : IMedicalCardService
{
    private IMedicalCardRepository medicalCardRepository;

    public MedicalCardService()
    {
        medicalCardRepository = new MedicalCardRepository();

    }

    public async Task<MedicalCard> GetMedicalCardByPatient(int patientId)
    {
        return await medicalCardRepository.GetMedicalCardByPatient(patientId);
    }

}