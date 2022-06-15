public interface IMedicalCardRepository
{
    public Task<MedicalCard> GetMedicalCard(int patientId);
}