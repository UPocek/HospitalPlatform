public interface IMedicalRecordRepository
{
    public Task UpdateMedicalRecord(int id, MedicalRecord medicalrecord);

    public Task UpdatePatientsPerscriptionList(int id, Prescription prescription);
    
    public Task<List<MedicalInstruction>> GetPrescriptions(int id);

    public Task<Prescription> GetPrescription(string drug, int id);
}