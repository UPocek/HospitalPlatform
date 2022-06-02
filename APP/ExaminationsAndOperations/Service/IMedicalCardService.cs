public interface IMedicalCardService
{
    public Task<MedicalCard> GetMedicalCard(int patientId);
}