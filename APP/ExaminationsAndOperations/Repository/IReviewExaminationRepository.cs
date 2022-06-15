public interface IReviewExaminationRepository
{
    public Task AddPerscription(int id, Prescription prescription);

    public Task AddMedicalInstruction(int id, MedicalInstruction medicalInstruction);

    public Task UpdatePatientsPerscriptionList(int id, Prescription prescription);
}