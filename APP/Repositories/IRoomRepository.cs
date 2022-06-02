public interface IRoomRepository{
    public Task<List<Room>> GetAllRooms();

    public  Task<Room> GetRoomByName(string roomName);

    public Task UpdateRoom(string name, Room room);
}