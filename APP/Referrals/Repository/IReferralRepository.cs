public interface IReferralRepository
{
    public Task CreateReferralForPatient(int id, Referral referral);
    public Task DeletePatientReferral(int referralid, Examination newExamination);
}