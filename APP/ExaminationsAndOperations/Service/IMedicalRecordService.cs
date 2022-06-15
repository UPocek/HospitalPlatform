public interface IMedicalRecordService
{
    public Task UpdateMedicalRecord(int id, MedicalRecord medicalRecord );

    public Task UpdatePatientsPerscriptionList(int id,Prescription prescription);

     public Task<bool> IsPerscriptionValid(int id, Prescription prescription);
}