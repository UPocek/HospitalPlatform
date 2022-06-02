public interface IRoomService
{
    public Task<List<Room>> GetAllRooms();

    public Task<Room> GetRoomByName(string roomName);

    public Task UpdateRoom(string name, Room room);

    public Task<bool> IsRoomNameValid(Room room);

    public Task SaveRoom(Room room);

    public Task DeleteRoom(string roomName);
}