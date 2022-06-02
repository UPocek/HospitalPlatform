public interface IPatientRepository
{
    public Task<List<Patient>> GetAllPatients();
    public Task<Patient> GetPatientById(int id);
}