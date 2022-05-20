using MongoDB.Driver;

public class ManagerService : IManagerService
{
    public bool IsRoomNameValid(IMongoCollection<Room> rooms, Room data)
    {
        return rooms.Find(item => item.Name == data.Name).FirstOrDefault() == null;
    }

    public bool IsDrugNameValid(IMongoCollection<Drug> drugs, Drug data)
    {
        return drugs.Find(item => item.Name == data.Name).FirstOrDefault() == null;
    }

    public bool IsIngredientNameValid(DrugIngredients allIngredients, Dictionary<string, string> data)
    {
        return !allIngredients.Ingredients.Contains(data["name"]);
    }

    public bool ExaminationScheduledAtThatTime(List<Examination> examinationsInRoom, Renovation data)
    {
        foreach (var examination in examinationsInRoom)
        {
            if (DateTime.Parse(examination.DateAndTimeOfExamination) >= DateTime.Parse(data.StartDate) && DateTime.Parse(examination.DateAndTimeOfExamination) <= DateTime.Parse(data.EndDate))
            {
                return true;
            }
        }
        return false;
    }

    public bool RenovationScheduledAtThatTime(List<Renovation> renovationsOnRoom, Renovation data)
    {
        foreach (var renovation in renovationsOnRoom)
        {
            if (DateTime.Parse(data.StartDate) <= DateTime.Parse(renovation.StartDate) && DateTime.Parse(data.EndDate) >= DateTime.Parse(renovation.StartDate) || DateTime.Parse(data.StartDate) >= DateTime.Parse(renovation.StartDate) && DateTime.Parse(data.StartDate) <= DateTime.Parse(renovation.EndDate))
            {
                return true;
            }
        }
        return false;
    }

    public void DevideEquipment(Room roomToBeDevided, List<Equipment> equipmentForRoom1, List<Equipment> equipmentForRoom2)
    {
        foreach (var el in roomToBeDevided.Equipment)
        {
            equipmentForRoom1.Add(new Equipment(el.Name, el.Type, el.Quantity / 2));
            equipmentForRoom2.Add(new Equipment(el.Name, el.Type, el.Quantity - el.Quantity / 2));
        }
    }

    public List<Equipment> GetMergedRoomsEquipment(Room room1, Room room2)
    {
        var mergedEquipment = new List<Equipment>();

        foreach (var el in room1.Equipment)
        {
            mergedEquipment.Add(el);
        }

        foreach (var el in room2.Equipment)
        {
            int index = mergedEquipment.FindIndex(item => item.Name == el.Name);
            if (index != -1)
            {
                mergedEquipment[index].Quantity += el.Quantity;
            }
            else
            {
                mergedEquipment.Add(el);
            }
        }

        return mergedEquipment;
    }
}