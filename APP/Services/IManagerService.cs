public interface IManagerService
{
    public Task<List<Room>> GetAllRooms();

    public Task<List<Drug>> GetAllDrugs();

    public Task<DrugIngredients> GetAllIngredients();

    public Task<Hospital> GetHospitalPolls();

    public Task<List<PollForDoctors>> GetDoctorPolls();

    public Task SaveRoom(Room room);

    public Task SaveRenovation(Renovation renovation);

    public Task SaveTransfer(Transfer transfer);

    public Task SaveDrug(Drug drug);

    public Task SaveIngredients(string ingredientName);

    public Task UpdateRoom(string id, Room room);

    public Task UpdateDrug(string id, Drug drug);

    public Task UpdateIngredients(string oldIngredient, string newIngredient);

    public Task DeleteRoom(string roomName);

    public Task DeleteDrug(string drugName);

    public Task DeleteIngredient(string ingredientName);

    public Task StartSimpleRenovation(Renovation renovation);

    public Task StartDevideRenovation(Renovation renovation);

    public Task StartMergeRenovation(Renovation renovation);

    public Task StartTransfer(Transfer transfer);

    public Task<bool> IsRoomNameValid(Room room);

    public Task<bool> IsDrugNameValid(Drug drug);

    public Task<bool> IngredientAlreadyExists(string name);

    public Task<bool> ExaminationScheduledAtThatTime(Renovation renovation);

    public Task<bool> RenovationScheduledAtThatTime(Renovation renovation);

}