using MongoDB.Driver;
public class RenovationService : CronJobService
{
    public RenovationService(IScheduleConfig<TransferService> config)
        : base(config.CronExpression, config.TimeZoneInfo)
    {
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        return base.StartAsync(cancellationToken);
    }

    public override Task DoWork(CancellationToken cancellationToken)
    {
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        var database = client.GetDatabase("USI");

        string dateToday = DateTime.UtcNow.ToString("yyyy-MM-dd");

        var collectionRenovations = database.GetCollection<Renovation>("Renovations");
        var simpleRenovations = collectionRenovations.Find(item => item.Done == false & item.Kind == "simple").ToList();

        foreach (var renovation in simpleRenovations)
        {
            if (dateToday == renovation.StartDate)
            {
                var collectionRooms = database.GetCollection<Room>("Rooms");
                var update = Builders<Room>.Update.Set("inRenovation", true);
                collectionRooms.UpdateMany(item => item.Name == renovation.Room, update);
            }
            if (dateToday == renovation.EndDate)
            {
                var collectionRooms = database.GetCollection<Room>("Rooms");
                var update = Builders<Room>.Update.Set("inRenovation", false);
                collectionRooms.UpdateMany(item => item.Name == renovation.Room, update);
            }
        }

        var devideRenovations = collectionRenovations.Find(item => item.StartDate == dateToday & item.Kind == "devide").ToList();

        foreach (var renovation in devideRenovations)
        {
            var collectionRooms = database.GetCollection<Room>("Rooms");
            var roomFilter = Builders<Room>.Filter.Eq("name", renovation.Room);
            var roomToBeDevided = collectionRooms.Find(roomFilter).FirstOrDefault();

            List<Equipment> equipmentForRoom1 = new List<Equipment>();
            List<Equipment> equipmentForRoom2 = new List<Equipment>();

            foreach (var el in roomToBeDevided.Equipment)
            {
                equipmentForRoom1.Add(new Equipment(el.Name, el.Type, el.Quantity / 2));
                equipmentForRoom2.Add(new Equipment(el.Name, el.Type, el.Quantity - el.Quantity / 2));
            }

            var room1 = new Room(roomToBeDevided.Name + ".1", roomToBeDevided.Type, true, equipmentForRoom1);
            var room2 = new Room(roomToBeDevided.Name + ".2", roomToBeDevided.Type, true, equipmentForRoom2);

            collectionRooms.InsertOne(room1);
            collectionRooms.InsertOne(room2);
            collectionRooms.DeleteOne(roomFilter);

            // Delete it also from everywhere elese
            var collectionTransfer = database.GetCollection<Transfer>("RelocationOfEquipment");
            collectionTransfer.DeleteMany(item => item.Room1 == renovation.Room | item.Room2 == renovation.Room);

            var collectionExamination = database.GetCollection<Examination>("MedicalExaminations");
            var updateRoom = Builders<Examination>.Update.Set("room", roomToBeDevided.Name + ".1");
            collectionExamination.UpdateMany(item => item.RoomName == renovation.Room, updateRoom);

            collectionRenovations.DeleteMany(item => item.Room == renovation.Room);

            var renovation1 = new Renovation(renovation.Room + ".1", renovation.StartDate, renovation.EndDate, false, "simple");
            var renovation2 = new Renovation(renovation.Room + ".2", renovation.StartDate, renovation.EndDate, false, "simple");

            collectionRenovations.InsertOne(renovation1);
            collectionRenovations.InsertOne(renovation2);
        }

        var mergeRenovations = collectionRenovations.Find(item => item.StartDate == dateToday & item.Kind == "merge").ToList();

        foreach (var renovation in mergeRenovations)
        {
            var collectionRooms = database.GetCollection<Room>("Rooms");
            var room1 = collectionRooms.Find(item => item.Name == renovation.Room).FirstOrDefault();
            var room2 = collectionRooms.Find(item => item.Name == renovation.Room2).FirstOrDefault();

            List<Equipment> equipmentOfNewRoom = new List<Equipment>();

            foreach (var el in room1.Equipment)
            {
                equipmentOfNewRoom.Add(el);
            }

            foreach (var el in room2.Equipment)
            {
                int index = equipmentOfNewRoom.FindIndex(item => item.Name == el.Name);
                if (index != -1)
                {
                    equipmentOfNewRoom[index].Quantity += el.Quantity;
                }
                else
                {
                    equipmentOfNewRoom.Add(el);
                }
            }

            var newRoom = new Room(room1.Name, room1.Type, true, equipmentOfNewRoom);

            collectionRooms.DeleteOne(item => item.Name == room1.Name);
            collectionRooms.DeleteOne(item => item.Name == room2.Name);
            collectionRooms.InsertOne(newRoom);

            // Delete them also from everywhere elese
            var collectionTransfer = database.GetCollection<Transfer>("RelocationOfEquipment");
            collectionTransfer.DeleteMany(item => item.Room1 == room1.Name | item.Room1 == room2.Name | item.Room2 == room1.Name | item.Room2 == room2.Name);

            var collectionExamination = database.GetCollection<Examination>("MedicalExaminations");
            var updateRoom = Builders<Examination>.Update.Set("room", room1.Name);
            collectionExamination.UpdateMany(item => item.RoomName == room2.Name, updateRoom);

            collectionRenovations.DeleteMany(item => item.Room == room1.Name | item.Room == room2.Name);

            var newRenovation = new Renovation(room1.Name, renovation.StartDate, renovation.EndDate, false, "simple");

            collectionRenovations.InsertOne(newRenovation);
        }

        var updateRenovation = Builders<Renovation>.Update.Set("done", true);
        collectionRenovations.UpdateMany(item => item.Done == false & item.Kind == "simple", updateRenovation);
        collectionRenovations.DeleteMany(item => item.StartDate == dateToday & item.Kind == "devide");
        collectionRenovations.DeleteMany(item => item.StartDate == dateToday & item.Kind == "merge");

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return base.StopAsync(cancellationToken);
    }
}