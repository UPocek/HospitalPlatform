public interface IMedicalCardRepository{
    public Task<MedicalCard> GetMedicalCardByPatient(int patientId);
}