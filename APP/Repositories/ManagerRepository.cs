using MongoDB.Driver;

public class ManagerRepository : IManagerRepository
{

    private IMongoDatabase database;

    public ManagerRepository()
    {

        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        database = client.GetDatabase("USI");

    }

    public async Task<List<Room>> GetAllRooms()
    {
        var rooms = database.GetCollection<Room>("Rooms");
        return await rooms.Find(item => true).ToListAsync();
    }

    public async Task<List<Drug>> GetAllDrugs()
    {
        var drugs = database.GetCollection<Drug>("Drugs");
        return await drugs.Find(item => true).ToListAsync();
    }

    public async Task<DrugIngredients> GetAllIngredients()
    {
        var drugIngredients = database.GetCollection<DrugIngredients>("DrugIngredients");
        return await drugIngredients.Find(item => true).FirstOrDefaultAsync();
    }

    public async Task<List<PollForDoctors>> GetAllDoctors()
    {
        var employees = database.GetCollection<PollForDoctors>("Employees");
        return await employees.Find(item => item.Role == "doctor").ToListAsync();
    }

    public async Task<List<Examination>> GetAllExaminationsInRoom(string roomName)
    {
        var medicalExaminations = database.GetCollection<Examination>("MedicalExaminations");
        return await medicalExaminations.Find(item => item.RoomName == roomName).ToListAsync();
    }

    public async Task<List<Renovation>> GetAllRenovationsInRoom(string roomName)
    {
        var renovations = database.GetCollection<Renovation>("Renovations");
        return await renovations.Find(item => item.Room == roomName).ToListAsync();
    }

    public async Task<Hospital> GetHospital()
    {
        var hospital = database.GetCollection<Hospital>("Hospital");
        return await hospital.Find(item => true).FirstOrDefaultAsync();
    }

    public async Task<Room> GetRoomByName(string name)
    {
        var rooms = database.GetCollection<Room>("Rooms");
        return await rooms.Find(item => item.Name == name).FirstOrDefaultAsync();
    }

    public async Task<Drug> GetDrugByName(string name)
    {
        var drugs = database.GetCollection<Drug>("Drugs");
        return await drugs.Find(item => item.Name == name).FirstOrDefaultAsync();
    }

    public async Task InsertRoom(Room room)
    {
        var rooms = database.GetCollection<Room>("Rooms");
        await rooms.InsertOneAsync(room);
    }

    public async Task InsertDrug(Drug drug)
    {
        var drugs = database.GetCollection<Drug>("Drugs");
        await drugs.InsertOneAsync(drug);
    }

    public async Task InsertIngredient(string ingredientName)
    {
        var drugIngredients = database.GetCollection<DrugIngredients>("DrugIngredients");
        var update = Builders<DrugIngredients>.Update.Push("ingredients", ingredientName);
        await drugIngredients.UpdateManyAsync(item => true, update);
    }

    public async Task InsertRenovation(Renovation renovation)
    {
        var renovations = database.GetCollection<Renovation>("Renovations");
        await renovations.InsertOneAsync(renovation);
    }

    public async Task InsertTransfer(Transfer transfer)
    {
        var transfers = database.GetCollection<Transfer>("RelocationOfEquipment");
        await transfers.InsertOneAsync(transfer);
    }

    public async Task DeleteRoom(string name)
    {
        var rooms = database.GetCollection<Room>("Rooms");
        await rooms.DeleteOneAsync(name);

        var transfers = database.GetCollection<Transfer>("RelocationOfEquipment");
        await transfers.DeleteManyAsync(item => item.Room1 == name | item.Room2 == name);

        var updateRoom = Builders<Examination>.Update.Set("room", name + ".1");
        var medicalExaminations = database.GetCollection<Examination>("MedicalExaminations");
        await medicalExaminations.UpdateManyAsync(item => item.RoomName == name, updateRoom);

        var renovations = database.GetCollection<Renovation>("Renovations");
        await renovations.DeleteManyAsync(item => item.Room == name);
    }

    public async Task DeleteDrug(string name)
    {
        var drugs = database.GetCollection<Drug>("Drugs");
        await drugs.DeleteOneAsync(item => item.Name == name);
    }

    public async Task DeleteIngredient(string name)
    {
        var drugIngredients = database.GetCollection<DrugIngredients>("DrugIngredients");
        var update = Builders<DrugIngredients>.Update.Pull("ingredients", name);
        await drugIngredients.UpdateManyAsync(item => true, update);
    }

    public async Task UpdateRoomInformation(string nameOfRoomToUpdate, string name, string type)
    {
        var rooms = database.GetCollection<Room>("Rooms");

        var filter = Builders<Room>.Filter.Eq("name", nameOfRoomToUpdate);

        var updateType = Builders<Room>.Update.Set("type", type);
        await rooms.UpdateOneAsync(filter, updateType);

        var updateName = Builders<Room>.Update.Set("name", name);
        await rooms.UpdateOneAsync(filter, updateName);

        var renovations = database.GetCollection<Renovation>("Renovations");
        var updateRenovations = Builders<Renovation>.Update.Set("room", name);
        await renovations.UpdateManyAsync(item => item.Room == nameOfRoomToUpdate, updateRenovations);

        var examinations = database.GetCollection<Examination>("MedicalExaminations");
        var updateExaminations = Builders<Examination>.Update.Set("room", name);
        await examinations.UpdateManyAsync(item => item.RoomName == nameOfRoomToUpdate, updateExaminations);

        var transfers = database.GetCollection<Transfer>("RelocationOfEquipment");
        var updateTransfer = Builders<Transfer>.Update.Set("room1", name);
        await transfers.UpdateManyAsync(item => item.Room1 == nameOfRoomToUpdate | item.Room2 == nameOfRoomToUpdate, updateTransfer);
    }

    public async Task UpdateDrugInformation(string nameOfDrugToUpdate, string name, List<string> ingredients, string status)
    {
        var drugs = database.GetCollection<Drug>("Drugs");

        var filter = Builders<Drug>.Filter.Eq("name", nameOfDrugToUpdate);

        var updateIngredients = Builders<Drug>.Update.Set("ingredients", ingredients);
        await drugs.UpdateOneAsync(filter, updateIngredients);

        var updateName = Builders<Drug>.Update.Set("name", name);
        await drugs.UpdateOneAsync(filter, updateName);

        var updateStatus = Builders<Drug>.Update.Set("status", status);
        await drugs.UpdateOneAsync(filter, updateStatus);
    }

    public async Task UpdateIngredientsInformation(string oldIngredient, string newIngredient)
    {
        await DeleteIngredient(oldIngredient);
        await InsertIngredient(newIngredient);
    }

    public async Task StartRenovationInRoom(string roomName)
    {
        var rooms = database.GetCollection<Room>("Rooms");
        var update = Builders<Room>.Update.Set("inRenovation", true);
        await rooms.UpdateManyAsync(item => item.Name == roomName, update);
    }

    public async Task MakeTransfer(string fromRoom, string toRoom, Equipment item)
    {
        var filter1 = Builders<Room>.Filter.Eq("name", fromRoom) & Builders<Room>.Filter.Eq("equipment.name", item.Name);
        var filter2 = Builders<Room>.Filter.Eq("name", toRoom) & Builders<Room>.Filter.Eq("equipment.name", item.Name);

        var rooms = database.GetCollection<Room>("Rooms");

        var update1 = Builders<Room>.Update.Inc("equipment.$.quantity", -1 * item.Quantity);
        await rooms.UpdateOneAsync(filter1, update1);

        // Increment quantity of selected equipment in room2 or add it if it is new
        if (await rooms.Find(filter2).FirstOrDefaultAsync() != null)
        {
            var update2 = Builders<Room>.Update.Inc("equipment.$.quantity", item.Quantity);
            await rooms.UpdateOneAsync(filter2, update2);
        }
        else
        {
            var update2 = Builders<Room>.Update.Push("equipment", item);
            await rooms.UpdateOneAsync(item => item.Name == toRoom, update2);
        }
    }

}