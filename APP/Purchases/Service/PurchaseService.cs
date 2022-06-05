public class PurchaseService : IPurchaseService
{
    private IPurchaseRepository _purchaseRepository;

    public PurchaseService()
    {
        _purchaseRepository = new PurchaseRepository();
    }

    public async Task CreateDynamicEquipmentPurchase(Equipment purchasedEquipment)
    {
        await _purchaseRepository.CreateDynamicEquipmentPurchase(purchasedEquipment);
    }

}