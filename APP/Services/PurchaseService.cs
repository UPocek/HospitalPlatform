using MongoDB.Driver;
public class PurchaseService : CronJobService
{
    public PurchaseService(IScheduleConfig<TransferService> config)
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

        var collectionPurchases = database.GetCollection<Purchase>("Purchases");
        string dateToday = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
        var purchases = collectionPurchases.Find(item => item.Deadline == dateToday & item.Done == false).ToList();

        var collectionRooms = database.GetCollection<Room>("Rooms");

        var mainWarehouse = collectionRooms.Find(r=>r.Name == "Main warehouse").FirstOrDefault();

        foreach (var purchase in purchases)
        {
            foreach(Equipment purchasedEquipment in purchase.What){
                for(int i=0;i<mainWarehouse.Equipment.Count();i++){
                    if (mainWarehouse.Equipment[i].Name == purchasedEquipment.Name){
                        mainWarehouse.Equipment[i].Quantity += purchasedEquipment.Quantity;
                    }
                }
            }
            collectionRooms.ReplaceOne(r=> r.Id == mainWarehouse.Id,mainWarehouse);
            purchase.Done = true;
            collectionPurchases.ReplaceOne(p=>p.id == purchase.id,purchase);

        }
        return Task.CompletedTask;
    }



    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return base.StopAsync(cancellationToken);
    }

}