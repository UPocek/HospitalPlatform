using MongoDB.Driver;
public class PurchaseRepository : IPurchaseRepository
{
    private IMongoDatabase _database;

    public PurchaseRepository()
    {
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        _database = client.GetDatabase("USI");
    }

    public async Task CreateDynamicEquipmentPurchase(Equipment purchasedEquipment)
    {
        Purchase newPurchase = new Purchase();
        newPurchase.Deadline = DateTime.Now.AddDays(1).ToString("yyyy-MM-ddTHH:mm");
        newPurchase.Done = false;
        newPurchase.What.Add(purchasedEquipment);

        var purchases = _database.GetCollection<Purchase>("Purchases");
        await purchases.InsertOneAsync(newPurchase);
    }

    public async Task<List<Purchase>> GetActivePurchases()
    {
        var collectionPurchases = _database.GetCollection<Purchase>("Purchases");
        string dateToday = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");

        return await collectionPurchases.Find(item => item.Deadline == dateToday & item.Done == false).ToListAsync();
    }

    public async Task UpdatePurchase(Purchase purchase){
        var collectionPurchases = _database.GetCollection<Purchase>("Purchases");
        await collectionPurchases.ReplaceOneAsync(p => p.id == purchase.id, purchase);
    }

}