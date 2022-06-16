using MongoDB.Driver;
public class CronPurchaseService : CronJobService
{
    IRoomRepository _roomRepository;

    IPurchaseRepository _purchaseRepository;
    public CronPurchaseService(IScheduleConfig<CronPurchaseService> config)
        : base(config.CronExpression, config.TimeZoneInfo)
    {
        _roomRepository = new RoomRepository();
        _purchaseRepository = new PurchaseRepository();
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        return base.StartAsync(cancellationToken);
    }

    public override async Task DoWork(CancellationToken cancellationToken)
    {
        List<Purchase> purchases = await _purchaseRepository.GetActivePurchases();

        Room mainWarehouse = await _roomRepository.GetRoomByName("Main warehouse");

        foreach (var purchase in purchases)
        {
            foreach (Equipment purchasedEquipment in purchase.What)
            {
                for (int i = 0; i < mainWarehouse.Equipment.Count(); i++)
                {
                    if (mainWarehouse.Equipment[i].Name == purchasedEquipment.Name)
                    {
                        mainWarehouse.Equipment[i].Quantity += purchasedEquipment.Quantity;
                    }
                }
            }
            await _roomRepository.UpdateRoom(mainWarehouse);
            purchase.Done = true;
            await _purchaseRepository.UpdatePurchase(purchase);

        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return base.StopAsync(cancellationToken);
    }

}