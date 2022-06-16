public class ReviewExaminationService : IReviewExaminationService
{
    private IReviewExaminationRepository _reviewExaminationRepository;

    private IMedicalCardRepository _medicalCardRepository;

    private IDrugRepository _drugRepository;

    public ReviewExaminationService()
    {
        _reviewExaminationRepository = new ReviewExaminationRepository();
        _medicalCardRepository = new MedicalCardRepository();
        _drugRepository = new DrugRepository();
    }

    public async Task AddPerscription(int id, Prescription prescription)
    {
        await _reviewExaminationRepository.AddPerscription(id, prescription);
    }

    public async Task AddMedicalInstruction(int id, MedicalInstruction medicalInstruction)
    {
        await _reviewExaminationRepository.AddMedicalInstruction(id, medicalInstruction);
    }

    public async Task UpdatePatientsPerscriptionList(int id, Prescription prescription)
    {
        await _reviewExaminationRepository.UpdatePatientsPerscriptionList(id, prescription);
    }

    public async Task<bool> IsPerscriptionValid(int id, Prescription perscription)
    {
        var medicalCard = await _medicalCardRepository.GetMedicalCard(id);
        var drugIngredients = await _drugRepository.GetDrugIngredients(perscription.DrugName);

        foreach (string ingredient in drugIngredients)
        {
            if (medicalCard.MedicalRecord.Alergies != null && medicalCard.MedicalRecord.Alergies.Contains(ingredient))
            {
                return false;
            }
        }

        return true;
    }


    public async Task<List<MedicalInstruction>> GetPrescriptions(int id)
    {
        return await _reviewExaminationRepository.GetPrescriptions(id);
    }
    public async Task<Prescription> GetPrescription(string drug, int id)
    {
        return await _reviewExaminationRepository.GetPrescription(drug, id);
    }
}