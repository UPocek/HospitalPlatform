using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class RenovationController
{

    private IRenovationService service;

    public RenovationController()
    {
        service = new RenovationService();
    }

}