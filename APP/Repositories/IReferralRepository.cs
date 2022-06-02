public interface IReferralRepository
{
    public Task CreateReferralForPatient(int id, Referral referral);
}