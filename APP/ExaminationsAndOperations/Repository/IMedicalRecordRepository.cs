public interface IMedicalRecordRepository
{
    public Task UpdateMedicalRecord(int id, MedicalRecord medicalrecord);

    public Task UpdatePatientsPerscriptionList(int id, Prescription prescription);
}