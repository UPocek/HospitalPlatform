public class RoomService : IRoomService
{
    private IRoomRepository _roomRepository;

    public RoomService()
    {
        _roomRepository = new RoomRepository();
    }

    public async Task<List<Room>> GetAllRooms()
    {
        return await _roomRepository.GetAllRooms();
    }

    public async Task<Room> GetRoomByName(string roomName)
    {
        return await _roomRepository.GetRoomByName(roomName);
    }

    public async Task UpdateRoom(string id, Room room)
    {
        await _roomRepository.UpdateRoomInformation(id, room.Name, room.Type);
    }

    public async Task<bool> IsRoomNameValid(Room room)
    {
        return await _roomRepository.GetRoomByName(room.Name) == null;
    }

    public async Task SaveRoom(Room room)
    {
        await _roomRepository.InsertRoom(room);
    }

    public async Task DeleteRoom(string roomName)
    {
        await _roomRepository.DeleteRoom(roomName);
    }

}