public interface IMedicalCardService
{
    public Task<MedicalCard> GetMedicalCardByPatient(int patientId);

}