using Microsoft.AspNetCore.Mvc;

public class UserService : IUserService
{
    private IUserRepository _userRepository;

    private readonly IJwtAuthenticationManager _manager;

    public UserService(IJwtAuthenticationManager manager)
    {
        _manager = manager;
        _userRepository = new UserRepository();
    }

    public IActionResult GetAllDoctors()
    {
        return  _userRepository.GetAllDoctors();
    }

    public async Task<Employee> GetDoctor(int doctorId)
    {
        return await _userRepository.GetDoctor(doctorId);
    }

    public IActionResult GetUser(int id)
    {
        if (id < 900)
        {
            return _userRepository.GetEmployee(id);
        }
        else
        {
            return _userRepository.GetPatient(id);
        }
    }

    public async Task<Account> Authenticate(string email, string password)
    {
        var user = await _userRepository.LoginEmployee(email, password);

        if (user != null)
        {
            var token = _manager.GenereteToken(email);
            var account = new Account(token, user);
            return account;
        }
        else
        {
            user = await _userRepository.LoginPatient(email, password);
            if (user != null && user.Active == "0")
            {
                var token = _manager.GenereteToken(email);
                var account = new Account(token, user);
                return account;
            }
        }
        return new Account();
    }

    public async Task<List<Employee>> GetSpecializedDoctors(string specialization){
        return await _userRepository.GetSpecializedDoctors(specialization);

    }

    public async Task<List<String>> GetDoctorSpecializations()
    {
        return await _userRepository.GetDoctorSpecializations();
    }
}