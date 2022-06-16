public interface IReferralService
{
    public Task CreateReferralForPatient(int id, Referral referral);

    public Task<int> FindReferralId(int patientId);

    public Task DeletePatientReferral(int id, Examination newExamination);

}