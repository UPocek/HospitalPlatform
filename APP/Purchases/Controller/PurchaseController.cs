#nullable disable
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

[Route("api/[controller]")]
[ApiController]
public class PurchaseController : ControllerBase
{
    private IPurchaseService _purchaseService;
    public PurchaseController()
    {
        _purchaseService = new PurchaseService();
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateDynamicEquipmentPurchase(Equipment purchasedEquipment)
    {
    await _purchaseService.CreateDynamicEquipmentPurchase(purchasedEquipment);

    return Ok();
    }


    

}
