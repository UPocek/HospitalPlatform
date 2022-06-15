public class RenovationService : IRenovationService
{
    private IRenovationRepository _renovationRepository;

    private IRoomService _roomService;

    public RenovationService()
    {
        _renovationRepository = new RenovationRepository();

        _roomService = new RoomService();
    }

    public async Task SaveRenovation(Renovation renovation)
    {
        await _renovationRepository.InsertRenovation(renovation);
    }

    public async Task<bool> RenovationScheduledAtThatTime(Renovation renovation)
    {
        List<Renovation> renovationsOnRoom = await _renovationRepository.GetAllRenovationsInRoom(renovation.Room);

        if (renovation.Room2 != null)
        {
            List<Renovation> renovationsOnRoom2 = await _renovationRepository.GetAllRenovationsInRoom(renovation.Room2);
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

    public async Task StartSimpleRenovation(Renovation renovation)
    {
        await _renovationRepository.StartRenovationInRoom(renovation.Room);
    }

    public async Task StartDevideRenovation(Renovation renovation)
    {
        var roomToBeDevided = await _roomService.GetRoomByName(renovation.Room);

        var equipmentForRoom1 = new List<Equipment>();
        var equipmentForRoom2 = new List<Equipment>();

        DevideEquipment(roomToBeDevided.Equipment, equipmentForRoom1, equipmentForRoom2);

        var room1 = new Room(roomToBeDevided.Name + ".1", roomToBeDevided.Type, true, equipmentForRoom1);
        var room2 = new Room(roomToBeDevided.Name + ".2", roomToBeDevided.Type, true, equipmentForRoom2);

        await _roomService.DeleteRoom(roomToBeDevided.Name);
        await _roomService.SaveRoom(room1);
        await _roomService.SaveRoom(room2);

        var renovation1 = new Renovation(roomToBeDevided.Name + ".1", renovation.StartDate, renovation.EndDate, false, "simple");
        var renovation2 = new Renovation(roomToBeDevided.Name + ".2", renovation.StartDate, renovation.EndDate, false, "simple");

        await SaveRenovation(renovation1);
        await SaveRenovation(renovation2);
    }

    public async Task StartMergeRenovation(Renovation renovation)
    {
        var room1 = await _roomService.GetRoomByName(renovation.Room);
        var room2 = await _roomService.GetRoomByName(renovation.Room2 != null ? renovation.Room2 : renovation.Room);

        var equipmentOfNewRoom = GetMergedRoomsEquipment(room1, room2);

        var newRoom = new Room(room1.Name, room1.Type, false, equipmentOfNewRoom);

        await _roomService.DeleteRoom(room1.Name);
        await _roomService.DeleteRoom(room2.Name);
        await _roomService.SaveRoom(newRoom);

        var renovationOnNewRoom = new Renovation(newRoom.Name, renovation.StartDate, renovation.EndDate, false, "simple");

        await SaveRenovation(renovationOnNewRoom);
    }

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