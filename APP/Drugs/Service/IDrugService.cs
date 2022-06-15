public interface IDrugService
{
    public Task<List<Drug>> GetAllDrugs();

    public Task<List<Drug>> GetAllDrugsForReview();

    public Task<List<string>> GetDrugIngredients(string name);

    public Task UpdateDrugMessage(string id, Dictionary<string, string> data);

    public Task ApproveDrug(string id);

    public Task<bool> IsDrugNameValid(Drug drug);

    public Task SaveDrug(Drug drug);

    public Task UpdateDrug(string id, Drug drug);

    public Task DeleteDrug(string drugName);
}