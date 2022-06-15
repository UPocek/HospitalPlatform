public interface IDynamicEquipmentRepository
{
    public Task<List<string>> GetExpendedDynamicEquipment();

    public Task<List<KeyValuePair<string, Equipment>>> GetLowDynamicEquipment();

    public Task<int> GetRoomEquipmentQuantity(string roomName, string equipmentName);
}