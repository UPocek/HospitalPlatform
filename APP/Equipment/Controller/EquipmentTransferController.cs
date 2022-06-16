#nullable disable
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class EquipmentTransferController : ControllerBase
{
    private IEquipmentTransferService _equipmentTransferService;
    private string _dateToday;

    public EquipmentTransferController()
    {
        _equipmentTransferService = new EquipmentTransferService();

        _dateToday = DateTime.Now.ToString("yyyy-MM-dd");
    }

    [HttpGet("{roomName}")]
    public async Task<List<Equipment>> GetRoomEquipment(String roomName)
    {
        return await _equipmentTransferService.GetRoomEquipment(roomName);
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateTransfer(Transfer transfer)
    {
        if (_dateToday == transfer.Date)
        {
            await _equipmentTransferService.StartTransfer(transfer);
            transfer.Done = true;
        }

        await _equipmentTransferService.SaveTransfer(transfer);

        return Ok();
    }

    [HttpPut("use")]
    public async Task<IActionResult> UseEquipment(Room room)
    {
        await _equipmentTransferService.UseEquipment(room);
        return Ok();
    }

    [HttpPut("dynamicTransfer/{equipmentName}/{fromRoomName}/{toRoomName}/{quantity}")]
    public async Task<IActionResult> TransferDynamicEquipment(string equipmentName, string fromRoomName, string toRoomName, int quantity)
    {
        await _equipmentTransferService.TransferDynamicEquipment(equipmentName, fromRoomName, toRoomName, quantity);
        return Ok();
    }

}