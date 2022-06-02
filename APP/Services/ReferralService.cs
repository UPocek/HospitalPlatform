public class ReferralService : IReferralService
{
    private IReferralRepository referralRepository;

    private IPatientRepository patientRepository;

    public ReferralService()
    {

        referralRepository = new ReferralRepository();
        patientRepository = new PatientRepository();


    }

    public async Task CreateReferralForPatient(int id, Referral referral)
    {
        await referralRepository.CreateReferralForPatient(id, referral);
    }

    public async Task<int> FindReferralId(int patientId)
    {
        var patient = await patientRepository.GetPatientById(patientId);
        if((int) patient.MedicalRecord.Referrals.Count() == 0){
            return 1;
        }else{
            var lastReferralId = (int) patient.MedicalRecord.Referrals.Last().ReferralId;
            return lastReferralId + 1;
        }      
    }

}