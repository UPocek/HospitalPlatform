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

    public async Task<Room> GetRoomByType(String type)
    {
        return await _roomRepository.GetRoomByType(type);
    }

    public async Task UpdateRoom(string id, Room room)
    {
        await _roomRepository.UpdateRoomInformation(id, room);
    }

    public async Task<bool> IsRoomNameValid(Room room)
    {
        return await _roomRepository.GetRoomByName(room.Name) == null;
    }

    public async Task SaveRoom(Room room)
    {
        await _roomRepository.InsertRoom(room);
    }

    public async Task UpdateRoom(Room room)
    {
        await _roomRepository.UpdateRoom(room);
    }

    public async Task DeleteRoom(string roomName)
    {
        await _roomRepository.DeleteRoom(roomName);
    }

}