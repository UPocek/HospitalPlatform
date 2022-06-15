using Microsoft.AspNetCore.Mvc;

public interface IUserService
{
    public IActionResult GetAllDoctors();

    public Task<Employee> GetDoctor(int doctorId);

    public IActionResult GetUser(int id);

    public Task<Account> Authenticate(string email, string password);

    public Task<List<Employee>> GetSpecializedDoctors(string specialization);

    public Task<List<String>> GetDoctorSpecializations();
}