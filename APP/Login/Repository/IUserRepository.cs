using Microsoft.AspNetCore.Mvc;

public interface IUserRepository
{
    public IActionResult GetAllDoctors();

    public IActionResult GetEmployee(int id);

    public Task<Employee> GetDoctor(int doctorId);

    public Task<List<Employee>> GetSpecializedDoctors(string specialization);

    public Task<List<String>> GetDoctorSpecializations();

    public IActionResult GetPatient(int patientId);

    public Task<User> LoginEmployee(string email, string password);

    public Task<User> LoginPatient(string email, string password);
}