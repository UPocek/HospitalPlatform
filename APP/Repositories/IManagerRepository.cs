public interface IManagerRepository
{
    public Task<List<Room>> GetAllRooms();

    public Task<List<Drug>> GetAllDrugs();

    public Task<DrugIngredients> GetAllIngredients();

    public Task<List<PollForDoctors>> GetAllDoctors();

    public Task<List<Examination>> GetAllExaminationsInRoom(string roomName);

    public Task<List<Renovation>> GetAllRenovationsInRoom(string roomName);

    public Task<Hospital> GetHospital();

    public Task<Room> GetRoomByName(string name);

    public Task<Drug> GetDrugByName(string name);

    public Task InsertRoom(Room room);

    public Task InsertRenovation(Renovation renovation);

    public Task InsertTransfer(Transfer transfer);

    public Task InsertDrug(Drug drug);

    public Task InsertIngredient(string ingredientName);

    public Task DeleteRoom(string name);

    public Task DeleteDrug(string name);

    public Task DeleteIngredient(string name);

    public Task UpdateRoomInformation(string nameOfRoomToUpdate, string name, string type);

    public Task UpdateDrugInformation(string nameOfDrugToUpdate, string name, List<string> ingredients, string status);

    public Task UpdateIngredientsInformation(string oldIngredient, string newIngredient);

    public Task StartRenovationInRoom(string roomName);

    public Task MakeTransfer(string fromRoom, string toRoom, Equipment item);

}