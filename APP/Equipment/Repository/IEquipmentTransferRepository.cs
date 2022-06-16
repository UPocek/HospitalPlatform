public interface IEquipmentTransferRepository
{
    public Task<List<Equipment>> GetRoomEquipment(String roomName);

    public Task InsertTransfer(Transfer transfer);

    public Task ExecuteTransfer(string fromRoom, string toRoom, Equipment item);
    public Task UseEquipment(Room room);
}