public interface IRoomRepository
{
    public Task<List<Room>> GetAllRooms();

    public Task<Room> GetRoomByName(string roomName);

    public Task UpdateRoomInformation(string nameOfRoomToUpdate, Room room);

    public Task InsertRoom(Room room);

    public Task DeleteRoom(string name);
}