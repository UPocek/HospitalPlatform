public interface IPurchaseRepository
{
    public Task CreateDynamicEquipmentPurchase(Equipment purchasedEquipment);

    public Task<List<Purchase>> GetActivePurchases();

    public Task UpdatePurchase(Purchase p);
}