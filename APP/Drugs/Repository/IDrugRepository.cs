public interface IDrugRepository
{
    public Task<List<Drug>> GetAllDrugs();

    public Task<List<string>> GetDrugIngredients(string name);

    public Task<List<Drug>> GetAllDrugsForReview();

    public Task UpdateDrugMessage(string id, Dictionary<string, string> data);

    public Task ApproveDrug(string id);

    public Task<Drug> GetDrugByName(string name);

    public Task InsertDrug(Drug drug);

    public Task UpdateDrugInformation(string nameOfDrugToUpdate, string name, List<string> ingredients, string status);

    public Task DeleteDrug(string name);
}