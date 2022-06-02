using MongoDB.Driver;

public class RoomRepository : IRoomRepository
{
    private IMongoDatabase database;

    public RoomRepository()
    {

        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        database = client.GetDatabase("USI");

    }
    public async Task<List<Room>> GetAllRooms()
    {

        var rooms = database.GetCollection<Room>("Rooms");
        return await rooms.Find(item => true).ToListAsync();

    }

    public async Task<Room> GetRoomByName(string roomName)
    {

        var rooms = database.GetCollection<Room>("Rooms");
        return await rooms.Find(room => room.Name == roomName).FirstOrDefaultAsync();

    }

    public async Task UpdateRoom(string name, Room room)
    {
        var rooms = database.GetCollection<Room>("Rooms");
        await rooms.FindOneAndReplaceAsync(r => r.Name == name, room);

    }

    public async Task InsertRoom(Room room)
    {
        var rooms = database.GetCollection<Room>("Rooms");
        await rooms.InsertOneAsync(room);
    }
}