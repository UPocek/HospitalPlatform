public interface IEquipmentTransferRepository
{
    public Task InsertTransfer(Transfer transfer);

    public Task ExecuteTransfer(string fromRoom, string toRoom, Equipment item);
}