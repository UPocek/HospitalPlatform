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

}