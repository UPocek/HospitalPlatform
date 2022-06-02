using MongoDB.Driver;
public class EquipmentTransferRepository : IEquipmentTransferRepository
{
    private IMongoDatabase _database;

    public EquipmentTransferRepository()
    {
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        _database = client.GetDatabase("USI");
    }

    public async Task InsertTransfer(Transfer transfer)
    {
        var transfers = _database.GetCollection<Transfer>("RelocationOfEquipment");
        await transfers.InsertOneAsync(transfer);
    }

    public async Task ExecuteTransfer(string fromRoom, string toRoom, Equipment item)
    {
        var filter1 = Builders<Room>.Filter.Eq("name", fromRoom) & Builders<Room>.Filter.Eq("equipment.name", item.Name);
        var filter2 = Builders<Room>.Filter.Eq("name", toRoom) & Builders<Room>.Filter.Eq("equipment.name", item.Name);

        var rooms = _database.GetCollection<Room>("Rooms");

        var update1 = Builders<Room>.Update.Inc("equipment.$.quantity", -1 * item.Quantity);
        await rooms.UpdateOneAsync(filter1, update1);

        // Increment quantity of selected equipment in room2 or add it if it is new
        if (await rooms.Find(filter2).FirstOrDefaultAsync() != null)
        {
            var update2 = Builders<Room>.Update.Inc("equipment.$.quantity", item.Quantity);
            await rooms.UpdateOneAsync(filter2, update2);
        }
        else
        {
            var update2 = Builders<Room>.Update.Push("equipment", item);
            await rooms.UpdateOneAsync(item => item.Name == toRoom, update2);
        }
    }

}