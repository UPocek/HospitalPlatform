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
    public async Task<List<Employee>> GetAllDoctors()
    {
        return await userService.GetAllDoctors();
    }

    [HttpGet("doctors/{id}")]
    public async Task<Employee> GetDoctor(int doctorId)
    {
        return await userService.GetDoctor(doctorId);
    }

    // GET: api/My/users/id
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
