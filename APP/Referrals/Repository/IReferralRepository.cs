public interface IReferralRepository
{
    public Task CreateReferralForPatient(int id, Referral referral);
    public void DeletePatientReferral(int referralid, Examination newExamination);
}