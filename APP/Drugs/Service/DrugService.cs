public class DrugService : IDrugService
{
    private IDrugRepository _drugRepository;

    public DrugService()
    {
        _drugRepository = new DrugRepository();
    }

    public async Task<List<Drug>> GetAllDrugs()
    {
        return await _drugRepository.GetAllDrugs();
    }

    public async Task<List<Drug>> GetAllDrugsForReview()
    {
        return await _drugRepository.GetAllDrugsForReview();
    }

    public async Task<List<string>> GetDrugIngredients(string name)
    {
        return await _drugRepository.GetDrugIngredients(name);
    }
    public async Task SaveDrug(Drug drug)
    {
        await _drugRepository.InsertDrug(drug);
    }

    public async Task UpdateDrugMessage(string id, Dictionary<string, string> data)
    {
        await _drugRepository.UpdateDrugMessage(id, data);
    }


    public async Task UpdateDrug(string id, Drug drug)
    {
        await _drugRepository.UpdateDrugInformation(id, drug.Name, drug.Ingredients, drug.Status);
    }

    public async Task DeleteDrug(string drugName)
    {
        await _drugRepository.DeleteDrug(drugName);
    }

    public async Task ApproveDrug(string id)
    {
        await _drugRepository.ApproveDrug(id);
    }

    public async Task<bool> IsDrugNameValid(Drug drug)
    {
        return await _drugRepository.GetDrugByName(drug.Name) == null;
    }

    
    public async Task CreateNotification(DrugNotification notification)
    {
        
        await _drugRepository.CreateNotification(notification);
    }

}