public class RoomService : IRoomService
{
    private IRoomRepository roomRepository;

    public RoomService()
    {

        roomRepository = new RoomRepository();

    }
    public async Task<List<Room>> GetAllRooms()
    {
        return await roomRepository.GetAllRooms();
    }
    public async Task<Room> GetRoomByName(string roomName)
    {
        return await roomRepository.GetRoomByName(roomName);
    }

    public async Task UpdateRoom(string name, Room room)
    {
        await roomRepository.UpdateRoom(name, room);
    }

    public async Task<bool> IsRoomNameValid(Room room)
    {
        return await roomRepository.GetRoomByName(room.Name) == null;
    }

    public async Task SaveRoom(Room room)
    {
        await roomRepository.InsertRoom(room);
    }
}