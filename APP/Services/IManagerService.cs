using MongoDB.Driver;

public interface IManagerService
{
    public bool IsRoomNameValid(IMongoCollection<Room> collection, Room data);

    public bool IsDrugNameValid(IMongoCollection<Drug> collection, Drug data);

    public bool IsIngredientNameValid(DrugIngredients allIngredients, Dictionary<string, string> data);

    public bool ExaminationScheduledAtThatTime(List<Examination> examinationsInRoom, Renovation data);

    public bool RenovationScheduledAtThatTime(List<Renovation> examinationsInRoom, Renovation data);

    public void DevideEquipment(Room roomToBeDevided, List<Equipment> equipmentForRoom1, List<Equipment> equipmentForRoom2);

    public List<Equipment> GetMergedRoomsEquipment(Room room1, Room room2);
}