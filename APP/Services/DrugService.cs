public class DrugService : IDrugService
{
    private IDrugRepository drugRepository;

    public DrugService()
    {
        drugRepository = new DrugRepository();

    }

    public async Task<List<Drug>> GetAllDrugs()
    {
        return await drugRepository.GetAllDrugs();
    }

    public async Task<List<Drug>> GetAllDrugsForReview()
    {
        return await drugRepository.GetAllDrugsForReview();
    }

    public async Task<List<string>> GetDrugsIngredients(string name)
    {
        return await drugRepository.GetDrugsIngredients(name);
    }

    public async Task UpdateDrugMessage(string id, Dictionary<string, string> data)
    {
        await drugRepository.UpdateDrugMessage(id, data);
    }

    public async Task ApproveDrug(string id)
    {
        await drugRepository.ApproveDrug(id);
    }
}