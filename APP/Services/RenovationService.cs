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
        var simpleRenovations = collectionRenovations.Find(item => item.done == false & item.kind == "simple").ToList();

        foreach (var renovation in simpleRenovations)
        {
            if (dateToday == renovation.startDate)
            {
                var collectionRooms = database.GetCollection<Room>("Rooms");
                var update = Builders<Room>.Update.Set("inRenovation", true);
                collectionRooms.UpdateMany(item => item.name == renovation.room, update);
            }
            if (dateToday == renovation.endDate)
            {
                var collectionRooms = database.GetCollection<Room>("Rooms");
                var update = Builders<Room>.Update.Set("inRenovation", false);
                collectionRooms.UpdateMany(item => item.name == renovation.room, update);
            }
        }

        var devideRenovations = collectionRenovations.Find(item => item.startDate == dateToday & item.kind == "devide").ToList();

        foreach (var renovation in devideRenovations)
        {
            var collectionRooms = database.GetCollection<Room>("Rooms");
            var roomFilter = Builders<Room>.Filter.Eq("name", renovation.room);
            var roomToBeDevided = collectionRooms.Find(roomFilter).FirstOrDefault();

            List<Equipment> equipmentForRoom1 = new List<Equipment>();
            List<Equipment> equipmentForRoom2 = new List<Equipment>();

            foreach (var el in roomToBeDevided.equipment)
            {
                equipmentForRoom1.Add(new Equipment(el.name, el.type, el.quantity / 2));
                equipmentForRoom2.Add(new Equipment(el.name, el.type, el.quantity - el.quantity / 2));
            }

            var room1 = new Room(roomToBeDevided.name + ".1", roomToBeDevided.type, true, equipmentForRoom1);
            var room2 = new Room(roomToBeDevided.name + ".2", roomToBeDevided.type, true, equipmentForRoom2);

            collectionRooms.InsertOne(room1);
            collectionRooms.InsertOne(room2);
            collectionRooms.DeleteOne(roomFilter);

            // Delete it also from everywhere elese
            var collectionTransfer = database.GetCollection<Transfer>("RelocationOfEquipment");
            collectionTransfer.DeleteMany(item => item.room1 == renovation.room | item.room2 == renovation.room);

            var collectionExamination = database.GetCollection<Examination>("MedicalExaminations");
            var updateRoom = Builders<Examination>.Update.Set("room", roomToBeDevided.name + ".1");
            collectionExamination.UpdateMany(item => item.roomName == renovation.room, updateRoom);

            collectionRenovations.DeleteMany(item => item.room == renovation.room);

            var renovation1 = new Renovation(renovation.room + ".1", renovation.startDate, renovation.endDate, false, "simple");
            var renovation2 = new Renovation(renovation.room + ".2", renovation.startDate, renovation.endDate, false, "simple");

            collectionRenovations.InsertOne(renovation1);
            collectionRenovations.InsertOne(renovation2);
        }

        var mergeRenovations = collectionRenovations.Find(item => item.startDate == dateToday & item.kind == "merge").ToList();

        foreach (var renovation in mergeRenovations)
        {
            var collectionRooms = database.GetCollection<Room>("Rooms");
            var room1 = collectionRooms.Find(item => item.name == renovation.room).FirstOrDefault();
            var room2 = collectionRooms.Find(item => item.name == renovation.room2).FirstOrDefault();

            List<Equipment> equipmentOfNewRoom = new List<Equipment>();

            foreach (var el in room1.equipment)
            {
                equipmentOfNewRoom.Add(el);
            }

            foreach (var el in room2.equipment)
            {
                int index = equipmentOfNewRoom.FindIndex(item => item.name == el.name);
                if (index != -1)
                {
                    equipmentOfNewRoom[index].quantity += el.quantity;
                }
                else
                {
                    equipmentOfNewRoom.Add(el);
                }
            }

            var newRoom = new Room(room1.name, room1.type, true, equipmentOfNewRoom);

            collectionRooms.DeleteOne(item => item.name == room1.name);
            collectionRooms.DeleteOne(item => item.name == room2.name);
            collectionRooms.InsertOne(newRoom);

            // Delete them also from everywhere elese
            var collectionTransfer = database.GetCollection<Transfer>("RelocationOfEquipment");
            collectionTransfer.DeleteMany(item => item.room1 == room1.name | item.room1 == room2.name | item.room2 == room1.name | item.room2 == room2.name);

            var collectionExamination = database.GetCollection<Examination>("MedicalExaminations");
            var updateRoom = Builders<Examination>.Update.Set("room", room1.name);
            collectionExamination.UpdateMany(item => item.roomName == room2.name, updateRoom);

            collectionRenovations.DeleteMany(item => item.room == room1.name | item.room == room2.name);

            var newRenovation = new Renovation(room1.name, renovation.startDate, renovation.endDate, false, "simple");

            collectionRenovations.InsertOne(newRenovation);
        }

        var updateRenovation = Builders<Renovation>.Update.Set("done", true);
        collectionRenovations.UpdateMany(item => item.done == false & item.kind == "simple", updateRenovation);
        collectionRenovations.DeleteMany(item => item.startDate == dateToday & item.kind == "devide");
        collectionRenovations.DeleteMany(item => item.startDate == dateToday & item.kind == "merge");

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return base.StopAsync(cancellationToken);
    }
}