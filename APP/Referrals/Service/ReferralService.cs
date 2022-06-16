public class ReferralService : IReferralService
{
    private IReferralRepository _referralRepository;

    private IPatientRepository _patientRepository;

    public ReferralService()
    {
        _referralRepository = new ReferralRepository();
        _patientRepository = new PatientRepository();
    }

    public async Task CreateReferralForPatient(int id, Referral referral)
    {
        await _referralRepository.CreateReferralForPatient(id, referral);
    }

    public async Task<int> FindReferralId(int patientId)
    {
        var patient = await _patientRepository.GetPatientById(patientId);

        if (patient.MedicalRecord.Referrals != null && patient.MedicalRecord.Referrals.Count() == 0)
        {
            return 1;
        }
        else
        {
            var lastReferralId = (int)patient.MedicalRecord.Referrals.Last().ReferralId;
            return lastReferralId + 1;
        }
    }

    public async Task DeletePatientReferral(int id, Examination newExamination)
    {
        await _referralRepository.DeletePatientReferral(id, newExamination);
    }

}