public class EquipmentTransferService : IEquipmentTransferService
{
    private IEquipmentTransferRepository _equipmentTransferRepository;

    public EquipmentTransferService()
    {
        _equipmentTransferRepository = new EquipmentTransferRepository();
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
}