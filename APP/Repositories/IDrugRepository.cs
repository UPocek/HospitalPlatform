public interface IDrugRepository{
    public Task<List<Drug>> GetAllDrugs();
    
    public Task<List<string>> GetDrugsIngredients(string name);

    public Task<List<Drug>> GetAllDrugsForReview();

    public Task UpdateDrugMessage(string id, Dictionary<string, string> data);

    public Task ApproveDrug(string id);
}