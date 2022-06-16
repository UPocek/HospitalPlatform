#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private IUserService userService;

    public UserController(IJwtAuthenticationManager jwtManager)
    {
        userService = new UserService(jwtManager);
    }

    [HttpGet("doctors")]
    public IActionResult GetAllDoctors()
    {
        return userService.GetAllDoctors();
        return Ok();
    }

    [HttpPost("doctors/specialized/{specialization}")]
    public async Task<List<Employee>> GetSpecializedDoctors(string specialization){
        return await userService.GetSpecializedDoctors(specialization);

    }

    [HttpGet("doctors/specializations")]
    public async Task<List<String>> GetDoctorSpecializations()
    {
        return await userService.GetDoctorSpecializations();
    }

    [HttpGet("doctors/{doctorId}")]
    public async Task<Employee> GetDoctor(int doctorId)
    {
        return await userService.GetDoctor(doctorId);
    }

    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        return userService.GetUser(id);
    }

    [AllowAnonymous]
    [HttpPost("login/{email}&{password}")]
    public async Task<Account> Authenticate(string email, string password)
    {
        return await userService.Authenticate(email, password);
    }

}
