#nullable disable
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class DynamicEquipmentController : ControllerBase
{
    private IDynamicEquipmentService _dynamicEquipmentService;
    private string _dateToday;

    public DynamicEquipmentController()
    {
        _dynamicEquipmentService = new DynamicEquipmentService();

    }

    [HttpGet("expended")]

    public async Task<List<string>> GetExpendedDynamicEquipment()
    {
        return await _dynamicEquipmentService.GetExpendedDynamicEquipment();
    }

    [HttpGet("low")]
    public async Task<List<KeyValuePair<string, Equipment>>> GetLowDynamicEquipment()
    {
        return await _dynamicEquipmentService.GetLowDynamicEquipment();
    }
    
    [HttpGet("quantity/{roomName}/{equipmentName}")]
    public async Task<int> GetRoomEquipmentQuantity(string roomName, string equipmentName)
    {
        return await _dynamicEquipmentService.GetRoomEquipmentQuantity(roomName,equipmentName);
    }


}