public interface IMedicalRecordService
{
    public Task UpdateMedicalRecord(int id, MedicalRecord medicalRecord );

    public Task UpdatePatientsPerscriptionList(int id,Prescription prescription);

    public Task<bool> IsPerscriptionValid(int id, Prescription prescription);
    public Task<List<MedicalInstruction>> GetPrescriptions(int id);

    public Task<Prescription> GetPrescription(string drug, int id);
}