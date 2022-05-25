using MongoDB.Driver;
public class TransferService : CronJobService
{
    public TransferService(IScheduleConfig<TransferService> config)
        : base(config.CronExpression, config.TimeZoneInfo)
    {
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        return base.StartAsync(cancellationToken);
    }

    public override Task DoWork(CancellationToken cancellationToken)
    {
        System.Console.WriteLine("Jebem ti mamu");
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        var database = client.GetDatabase("USI");

        var collectionTransfers = database.GetCollection<Transfer>("RelocationOfEquipment");
        string dateToday = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var transfers = collectionTransfers.Find(item => item.Date == dateToday & item.Done == false).ToList();

        var collection = database.GetCollection<Room>("Rooms");

        foreach (var transfer in transfers)
        {
            foreach (var item in transfer.Equipment)
            {
                var filter1 = Builders<Room>.Filter.Eq("name", transfer.Room1) & Builders<Room>.Filter.Eq("equipment.name", item.Name);
                var filter2 = Builders<Room>.Filter.Eq("name", transfer.Room2) & Builders<Room>.Filter.Eq("equipment.name", item.Name);

                var update1 = Builders<Room>.Update.Inc("equipment.$.quantity", -1 * item.Quantity);
                collection.UpdateOne(filter1, update1);

                if (collection.Find(filter2).FirstOrDefault() != null)
                {
                    var update2 = Builders<Room>.Update.Inc("equipment.$.quantity", item.Quantity);
                    collection.UpdateOne(filter2, update2);
                }
                else
                {
                    var update2 = Builders<Room>.Update.Push("equipment", item);
                    collection.UpdateOne(item => item.Name == transfer.Room2, update2);
                }
            }
        }
        var updateTransfer = Builders<Transfer>.Update.Set("done", true);
        collectionTransfers.UpdateMany(item => item.Date == dateToday & item.Done == false, updateTransfer);
        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return base.StopAsync(cancellationToken);
    }
}