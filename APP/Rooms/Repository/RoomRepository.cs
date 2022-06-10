using MongoDB.Driver;

public class RoomRepository : IRoomRepository
{
    private IMongoDatabase _database;

    public RoomRepository()
    {
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        _database = client.GetDatabase("USI");
    }

    public async Task<List<Room>> GetAllRooms()
    {
        var rooms = _database.GetCollection<Room>("Rooms");
        return await rooms.Find(item => true).ToListAsync();
    }

    public async Task<Room> GetRoomByName(string roomName)
    {
        var rooms = _database.GetCollection<Room>("Rooms");
        return await rooms.Find(room => room.Name == roomName).FirstOrDefaultAsync();
    }

    public async Task UpdateRoomInformation(string nameOfRoomToUpdate, Room room)
    {
        var rooms = _database.GetCollection<Room>("Rooms");

        var filter = Builders<Room>.Filter.Eq("name", nameOfRoomToUpdate);

        var updateType = Builders<Room>.Update.Set("type", room.Type);
        await rooms.UpdateOneAsync(filter, updateType);

        var updateName = Builders<Room>.Update.Set("name", room.Name);
        await rooms.UpdateOneAsync(filter, updateName);

        var renovations = _database.GetCollection<Renovation>("Renovations");
        var updateRenovations = Builders<Renovation>.Update.Set("room", room.Name);
        await renovations.UpdateManyAsync(item => item.Room == nameOfRoomToUpdate, updateRenovations);

        var examinations = _database.GetCollection<Examination>("MedicalExaminations");
        var updateExaminations = Builders<Examination>.Update.Set("room", room.Name);
        await examinations.UpdateManyAsync(item => item.RoomName == nameOfRoomToUpdate, updateExaminations);

        var transfers = _database.GetCollection<Transfer>("RelocationOfEquipment");
        var updateTransfer = Builders<Transfer>.Update.Set("room1", room.Name);
        await transfers.UpdateManyAsync(item => item.Room1 == nameOfRoomToUpdate | item.Room2 == nameOfRoomToUpdate, updateTransfer);
    }

    public async Task InsertRoom(Room room)
    {
        var rooms = _database.GetCollection<Room>("Rooms");
        await rooms.InsertOneAsync(room);
    }

    public async Task DeleteRoom(string name)
    {
        var rooms = _database.GetCollection<Room>("Rooms");
        await rooms.DeleteOneAsync(name);

        var transfers = _database.GetCollection<Transfer>("RelocationOfEquipment");
        await transfers.DeleteManyAsync(item => item.Room1 == name | item.Room2 == name);

        var updateRoom = Builders<Examination>.Update.Set("room", name + ".1");
        var medicalExaminations = _database.GetCollection<Examination>("MedicalExaminations");
        await medicalExaminations.UpdateManyAsync(item => item.RoomName == name, updateRoom);

        var renovations = _database.GetCollection<Renovation>("Renovations");
        await renovations.DeleteManyAsync(item => item.Room == name);
    }

}