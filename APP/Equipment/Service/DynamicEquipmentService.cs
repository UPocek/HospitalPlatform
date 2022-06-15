public class DynamicEquipmentService : IDynamicEquipmentService
{
    private IDynamicEquipmentRepository _dynamicEquipmentRepository;

    public DynamicEquipmentService()
    {
        _dynamicEquipmentRepository = new DynamicEquipmentRepository();
    }

    public async Task<List<string>> GetExpendedDynamicEquipment()
    {
        return await _dynamicEquipmentRepository.GetExpendedDynamicEquipment();
    }

    public async Task<List<KeyValuePair<string, Equipment>>> GetLowDynamicEquipment()
    {
        return await _dynamicEquipmentRepository.GetLowDynamicEquipment();
    }
    
    public async Task<int> GetRoomEquipmentQuantity(string roomName, string equipmentName)
    {
        return await _dynamicEquipmentRepository.GetRoomEquipmentQuantity(roomName,equipmentName);
    }


}