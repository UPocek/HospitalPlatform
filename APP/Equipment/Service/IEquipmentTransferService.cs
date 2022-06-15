public interface IEquipmentTransferService
{
    public Task SaveTransfer(Transfer transfer);

    public Task StartTransfer(Transfer transfer);

    public Task UseEquipment(Room room);

    public Task TransferDynamicEquipment(string equipmentName, string fromRoomName, string toRoomName, int quantity);

}