using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class RoomController : ControllerBase
{
    private IRoomService _roomService;

    public RoomController()
    {
        _roomService = new RoomService();
    }

    [HttpGet("")]
    public async Task<List<Room>> GetRooms()
    {
        return await _roomService.GetAllRooms();
    }

    [HttpGet("{roomName}")]
    public async Task<Room> GetRoomByName(string roomName)
    {
        return await _roomService.GetRoomByName(roomName);
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateRoom(Room room)
    {
        if (!await _roomService.IsRoomNameValid(room))
        {
            return BadRequest();
        }

        await _roomService.SaveRoom(room);

        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRoom(string id, Room room)
    {
        if (!await _roomService.IsRoomNameValid(room))
        {
            return BadRequest();
        }

        await _roomService.UpdateRoom(id, room);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoom(string id)
    {
        await _roomService.DeleteRoom(id);

        return Ok();
    }

}