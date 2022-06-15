public class PatientService : IPatientService
{
    private IPatientRepository _patientRepository;

    public PatientService()
    {
        _patientRepository = new PatientRepository();
    }

    public async Task<List<Patient>> GetAllPatients()
    {
        return await _patientRepository.GetAllPatients();
    }

    public async Task<Patient> GetPatientById(int id)
    {
        return await _patientRepository.GetPatientById(id);
    }

    public async Task<List<Patient>> GetUnblockedPatients()
    {
        return await _patientRepository.GetUnblockedPatients();
    }

    public async Task<Patient> GetUnblockedPatient(int id)
    {
       return await _patientRepository.GetUnblockedPatient(id);
    }

    public async Task<List<Patient>> GetBlockedPatients()
    {
        return await _patientRepository.GetBlockedPatients();
    }
    public async Task<String> GetPatientActivity(int id)
    {
        return await _patientRepository.GetPatientActivity(id);
    }

    public async Task<bool> isNewPatientValid(Patient p){
        List<Patient> patients =  await _patientRepository.GetAllPatients();

        foreach(Patient p1 in patients){
            if(p1.Email == p.Email){
                return false;
            }
        }

        return true;

    }

    // POST: api/Secretary/patients
    //[HttpPost("patients")]
    public async Task CreatePatient(Patient patient)
    {
        await _patientRepository.CreatePatient(patient);
    }

    // POST: api/Secretary/patients/901
    //[HttpPut("patients/{id}")]
    public async Task UpdatePatient(int id, Patient patient)
    {
        await _patientRepository.UpdatePatient(id , patient);
    }

    // DELETE: api/Secretary/patients/901

    //[HttpDelete("patients/{id}")]
    public async Task DeletePatient(int id)
    {
        await _patientRepository.DeletePatient(id);

    }

    //[HttpPut("patients/block/{id}/{activityValue}")]
    // PUT: api/Secretary/patients/901/1
    public async Task UpdatePatientActivity(int id, string activityValue)
    {
        await _patientRepository.UpdatePatientActivity(id,activityValue);
    }

    public async Task<bool> PatientExists(int id)
    {
        return await _patientRepository.PatientExists(id);
    }

}