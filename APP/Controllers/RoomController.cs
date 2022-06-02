using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class RoomController : ControllerBase
{
    private IRoomService service;
    private string dateToday;

    public RoomController()
    {
        service = new RoomService();

        dateToday = DateTime.Now.ToString("yyyy-MM-dd");
    }

    [HttpGet("all")]
    public async Task<List<Room>> GetRooms()
    {
        return await service.GetAllRooms();
    }

    [HttpGet("{roomName}")]
    public async Task<Room> GetRoomByName(string roomName)
    {
        return await service.GetRoomByName(roomName);
    }

    [HttpPut("{roomName}")]
    public async Task<IActionResult> UpdateRoom(string name, Room room)
    {
        await service.UpdateRoom(name, room);
        return Ok();    
    }
}