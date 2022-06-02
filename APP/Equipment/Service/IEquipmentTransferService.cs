public interface IEquipmentTransferService
{
    public Task SaveTransfer(Transfer transfer);

    public Task StartTransfer(Transfer transfer);
}