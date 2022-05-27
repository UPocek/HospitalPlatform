public class ManagerService : IManagerService
{
    private IManagerRepository repository;
    public ManagerService()
    {
        repository = new ManagerRepository();
    }

    public async Task<List<Room>> GetAllRooms()
    {
        return await repository.GetAllRooms();
    }

    public async Task<List<Drug>> GetAllDrugs()
    {
        return await repository.GetAllDrugs();
    }

    public async Task<DrugIngredients> GetAllIngredients()
    {
        return await repository.GetAllIngredients();
    }

    public async Task<Hospital> GetHospitalPolls()
    {
        return await repository.GetHospital();
    }

    public async Task<List<PollForDoctors>> GetDoctorPolls()
    {
        return await repository.GetAllDoctors();
    }

    public async Task SaveRoom(Room room)
    {
        await repository.InsertRoom(room);
    }

    public async Task SaveRenovation(Renovation renovation)
    {
        await repository.InsertRenovation(renovation);
    }

    public async Task SaveTransfer(Transfer transfer)
    {
        await repository.InsertTransfer(transfer);
    }

    public async Task SaveDrug(Drug drug)
    {
        await repository.InsertDrug(drug);
    }

    public async Task SaveIngredients(string ingredientName)
    {
        await repository.InsertIngredient(ingredientName);
    }

    public async Task UpdateRoom(string id, Room room)
    {
        await repository.UpdateRoomInformation(id, room.Name, room.Type);
    }

    public async Task UpdateDrug(string id, Drug drug)
    {
        await repository.UpdateDrugInformation(id, drug.Name, drug.Ingredients, drug.Status);
    }

    public async Task UpdateIngredients(string oldIngredient, string newIngredient)
    {
        await repository.UpdateIngredientsInformation(oldIngredient, newIngredient);
    }

    public async Task DeleteRoom(string roomName)
    {
        await repository.DeleteRoom(roomName);
    }

    public async Task DeleteDrug(string drugName)
    {
        await repository.DeleteDrug(drugName);
    }

    public async Task DeleteIngredient(string ingredientName)
    {
        await repository.DeleteIngredient(ingredientName);
    }

    public async Task StartSimpleRenovation(Renovation renovation)
    {
        await repository.StartRenovationInRoom(renovation.Room);
    }

    public async Task StartDevideRenovation(Renovation renovation)
    {
        var roomToBeDevided = await repository.GetRoomByName(renovation.Room);

        var equipmentForRoom1 = new List<Equipment>();
        var equipmentForRoom2 = new List<Equipment>();

        DevideEquipment(roomToBeDevided.Equipment, equipmentForRoom1, equipmentForRoom2);

        var room1 = new Room(roomToBeDevided.Name + ".1", roomToBeDevided.Type, true, equipmentForRoom1);
        var room2 = new Room(roomToBeDevided.Name + ".2", roomToBeDevided.Type, true, equipmentForRoom2);

        await DeleteRoom(roomToBeDevided.Name);
        await SaveRoom(room1);
        await SaveRoom(room2);

        var renovation1 = new Renovation(roomToBeDevided.Name + ".1", renovation.StartDate, renovation.EndDate, false, "simple");
        var renovation2 = new Renovation(roomToBeDevided.Name + ".2", renovation.StartDate, renovation.EndDate, false, "simple");

        await SaveRenovation(renovation1);
        await SaveRenovation(renovation2);
    }

    public async Task StartMergeRenovation(Renovation renovation)
    {
        var room1 = await repository.GetRoomByName(renovation.Room);
        var room2 = await repository.GetRoomByName(renovation.Room2 != null ? renovation.Room2 : renovation.Room);

        var equipmentOfNewRoom = GetMergedRoomsEquipment(room1, room2);

        var newRoom = new Room(room1.Name, room1.Type, false, equipmentOfNewRoom);

        await DeleteRoom(room1.Name);
        await DeleteRoom(room2.Name);
        await SaveRoom(newRoom);

        var renovationOnNewRoom = new Renovation(newRoom.Name, renovation.StartDate, renovation.EndDate, false, "simple");

        await SaveRenovation(renovationOnNewRoom);
    }

    public async Task StartTransfer(Transfer transfer)
    {
        foreach (var item in transfer.Equipment)
        {
            await repository.MakeTransfer(transfer.Room1, transfer.Room2, item);
        }
    }

    public async Task<bool> IsRoomNameValid(Room room)
    {
        return await repository.GetRoomByName(room.Name) == null;
    }

    public async Task<bool> IsDrugNameValid(Drug drug)
    {
        return await repository.GetDrugByName(drug.Name) == null;
    }

    public async Task<bool> IngredientAlreadyExists(string name)
    {
        var allIngredients = await GetAllIngredients();
        return allIngredients.Ingredients.Contains(name);
    }

    public async Task<bool> ExaminationScheduledAtThatTime(Renovation renovation)
    {
        List<Examination> examinations = await repository.GetAllExaminationsInRoom(renovation.Room);
        if (renovation.Room2 != null)
        {
            List<Examination> examinations2 = await repository.GetAllExaminationsInRoom(renovation.Room2);
            examinations = examinations.Concat(examinations2).ToList();
        }
        foreach (var examination in examinations)
        {
            bool examinationOverlapsRenovation = DateTime.Parse(examination.DateAndTimeOfExamination) >= DateTime.Parse(renovation.StartDate) && DateTime.Parse(examination.DateAndTimeOfExamination) <= DateTime.Parse(renovation.EndDate);
            if (examinationOverlapsRenovation)
            {
                return true;
            }
        }
        return false;
    }

    public async Task<bool> RenovationScheduledAtThatTime(Renovation renovation)
    {
        List<Renovation> renovationsOnRoom = await repository.GetAllRenovationsInRoom(renovation.Room);
        if (renovation.Room2 != null)
        {
            List<Renovation> renovationsOnRoom2 = await repository.GetAllRenovationsInRoom(renovation.Room2);
            renovationsOnRoom = renovationsOnRoom.Concat(renovationsOnRoom2).ToList();
        }
        foreach (var r in renovationsOnRoom)
        {
            bool renovationAlreadyScheduled = DateTime.Parse(renovation.StartDate) <= DateTime.Parse(r.StartDate) && DateTime.Parse(renovation.EndDate) >= DateTime.Parse(r.StartDate) || DateTime.Parse(renovation.StartDate) >= DateTime.Parse(r.StartDate) && DateTime.Parse(renovation.StartDate) <= DateTime.Parse(r.EndDate);
            if (renovationAlreadyScheduled)
            {
                return true;
            }
        }
        return false;
    }

    // Helper methods

    private void DevideEquipment(List<Equipment> equipmentOfRoomToBeDevided, List<Equipment> equipmentForRoom1, List<Equipment> equipmentForRoom2)
    {
        foreach (var el in equipmentOfRoomToBeDevided)
        {
            equipmentForRoom1.Add(new Equipment(el.Name, el.Type, el.Quantity / 2));
            equipmentForRoom2.Add(new Equipment(el.Name, el.Type, el.Quantity - el.Quantity / 2));
        }
    }

    private List<Equipment> GetMergedRoomsEquipment(Room room1, Room room2)
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