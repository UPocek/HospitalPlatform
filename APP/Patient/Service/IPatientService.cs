public interface IPatientService
{
    public Task<List<Patient>> GetAllPatients();
    public Task<Patient> GetPatientById(int id);
    public Task<List<Patient>> GetUnblockedPatients();
    public Task<Patient> GetUnblockedPatient(int id);
    public Task<List<Patient>> GetBlockedPatients();
    public Task<String> GetPatientActivity(int id);
    public Task CreatePatient(Patient patient);
    public Task UpdatePatient(int id, Patient patient);
    public Task DeletePatient(int id);
    public Task UpdatePatientActivity(int id, string activityValue);
    public Task<bool> isNewPatientValid(Patient p);
    public Task<bool> PatientExists(int id);

}