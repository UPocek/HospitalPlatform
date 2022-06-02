using Microsoft.AspNetCore.Mvc;

public interface IUserRepository
{
    public Task<List<Employee>> GetAllDoctors();

    public IActionResult GetEmployee(int id);

    public Task<Employee> GetDoctor(int doctorId);

    public IActionResult GetPatient(int patientId);

    public Task<User> LoginEmployee(string email, string password);

    public Task<User> LoginPatient(string email, string password);
}