public interface IEquipmentTransferService
{
    public Task SaveTransfer(Transfer transfer);

    public Task StartTransfer(Transfer transfer);

    public Task UseEquipment(Room room);
}