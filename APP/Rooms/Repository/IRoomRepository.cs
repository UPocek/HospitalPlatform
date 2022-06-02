public interface IRoomRepository
{
    public Task<List<Room>> GetAllRooms();

    public Task<Room> GetRoomByName(string roomName);

    public Task UpdateRoomInformation(string nameOfRoomToUpdate, string name, string type);

    public Task InsertRoom(Room room);

    public Task DeleteRoom(string name);
}