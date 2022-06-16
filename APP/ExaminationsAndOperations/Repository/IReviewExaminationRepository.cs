public interface IReviewExaminationRepository
{
    public Task AddPerscription(int id, Prescription prescription);

    public Task AddMedicalInstruction(int id, MedicalInstruction medicalInstruction);

    public Task UpdatePatientsPerscriptionList(int id, Prescription prescription);

    public Task<List<MedicalInstruction>> GetPrescriptions(int id);

    public Task<Prescription> GetPrescription(string drug, int id);
}