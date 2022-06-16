public class EquipmentTransferService : IEquipmentTransferService
{
    private IEquipmentTransferRepository _equipmentTransferRepository;
    private IRoomRepository _roomRepository;

    public EquipmentTransferService()
    {
        _equipmentTransferRepository = new EquipmentTransferRepository();
        _roomRepository = new RoomRepository();
    }

    public async Task<List<Equipment>> GetRoomEquipment(String roomName)
    {
        return await _equipmentTransferRepository.GetRoomEquipment(roomName);
    }

    public async Task SaveTransfer(Transfer transfer)
    {
        await _equipmentTransferRepository.InsertTransfer(transfer);
    }

    public async Task StartTransfer(Transfer transfer)
    {
        foreach (var item in transfer.Equipment)
        {
            await _equipmentTransferRepository.ExecuteTransfer(transfer.Room1, transfer.Room2, item);
        }
    }

    public async Task UseEquipment(Room room)
    {
        await _equipmentTransferRepository.UseEquipment(room);
    }

    public async Task TransferDynamicEquipment(string equipmentName, string fromRoomName, string toRoomName, int quantity)
    {

        var transferFromRoom = await _roomRepository.GetRoomByName(fromRoomName);

        var transferToRoom = await _roomRepository.GetRoomByName(toRoomName);

        foreach (Equipment fromRoomEquipment in transferFromRoom.Equipment)
        {
            if (fromRoomEquipment.Name == equipmentName)
            {
                fromRoomEquipment.Quantity -= quantity;
                await _roomRepository.UpdateRoom(transferFromRoom);
            }
        }

        foreach (Equipment toRoomEquipment in transferToRoom.Equipment)
        {
            if (toRoomEquipment.Name == equipmentName)
            {
                toRoomEquipment.Quantity += quantity;
                await _roomRepository.UpdateRoom(transferToRoom);
            }
        }
    }
}