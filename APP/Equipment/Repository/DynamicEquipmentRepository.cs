using MongoDB.Driver;
public class DynamicEquipmentRepository : IDynamicEquipmentRepository
{
    private IMongoDatabase _database;

    public DynamicEquipmentRepository()
    {
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        _database = client.GetDatabase("USI");
    }

    public async Task<List<string>> GetExpendedDynamicEquipment()
    {
        var rooms = _database.GetCollection<Room>("Rooms");

        Dictionary<string, int> dynamicEquipmentQuantity = new Dictionary<string, int>();

        foreach (Room r in await rooms.Find(item => true).ToListAsync())
        {
            foreach (Equipment e in r.Equipment)
            {
                if (e.Type == "operation equipment")
                {
                    int oldQuantity;
                    if (dynamicEquipmentQuantity.TryGetValue(e.Name, out oldQuantity))
                    {
                        dynamicEquipmentQuantity[e.Name] = oldQuantity + e.Quantity;
                    }
                    else
                    {
                        dynamicEquipmentQuantity.Add(e.Name, e.Quantity);
                    }
                }
            }
        }

        List<string> expendedDynamicEquipment = new List<string>();

        foreach (KeyValuePair<string, int> equipmentQuantityEntry in dynamicEquipmentQuantity)
        {
            if (equipmentQuantityEntry.Value == 0)
            {
                expendedDynamicEquipment.Add(equipmentQuantityEntry.Key);
            }
        }

        return expendedDynamicEquipment;
    }



    public async Task<List<KeyValuePair<string, Equipment>>> GetLowDynamicEquipment()
    {
        var rooms = _database.GetCollection<Room>("Rooms");

        List<KeyValuePair<string, Equipment>> lowDynamicEquipment = new List<KeyValuePair<string, Equipment>>();

        foreach (Room r in await rooms.Find(item => item.Name != "Main warehouse").ToListAsync())
        {
            foreach (Equipment e in r.Equipment)
            {
                if (e.Type == "operation equipment" && e.Quantity <= 5)
                {
                    lowDynamicEquipment.Add(new KeyValuePair<string, Equipment>(r.Name, e));
                }
            }
        }

        lowDynamicEquipment.Sort((x, y) => x.Value.Quantity.CompareTo(y.Value.Quantity));

        return lowDynamicEquipment;
    }
    
    public async Task<int> GetRoomEquipmentQuantity(string roomName, string equipmentName)
    {
        var rooms = _database.GetCollection<Room>("Rooms");

        var room = await rooms.Find(r => r.Name == roomName).FirstOrDefaultAsync();

        foreach (Equipment roomEquipment in room.Equipment)
        {
            if (roomEquipment.Name == equipmentName)
            {
                return roomEquipment.Quantity;
            }
        }

        return 0;
    }

}