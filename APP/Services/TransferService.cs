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
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        var database = client.GetDatabase("USI");

        var collectionTransfers = database.GetCollection<Transfer>("RelocationOfEquipment");
        string dateToday = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var transfers = collectionTransfers.Find(item => item.date == dateToday & item.done == false).ToList();

        var collection = database.GetCollection<Room>("Rooms");

        foreach (var transfer in transfers)
        {
            foreach (var item in transfer.equipment)
            {
                var filter1 = Builders<Room>.Filter.Eq("name", transfer.room1) & Builders<Room>.Filter.Eq("equipment.name", item.name);
                var filter2 = Builders<Room>.Filter.Eq("name", transfer.room2) & Builders<Room>.Filter.Eq("equipment.name", item.name);

                var update1 = Builders<Room>.Update.Inc("equipment.$.quantity", -1 * item.quantity);
                collection.UpdateOne(filter1, update1);

                if (collection.Find(filter2).FirstOrDefault() != null)
                {
                    var update2 = Builders<Room>.Update.Inc("equipment.$.quantity", item.quantity);
                    collection.UpdateOne(filter2, update2);
                }
                else
                {
                    var update2 = Builders<Room>.Update.Push("equipment", item);
                    collection.UpdateOne(item => item.name == transfer.room2, update2);
                }
            }
        }
        var updateTransfer = Builders<Transfer>.Update.Set("done", true);
        collectionTransfers.UpdateMany(item => item.date == dateToday & item.done == false, updateTransfer);
        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return base.StopAsync(cancellationToken);
    }
}