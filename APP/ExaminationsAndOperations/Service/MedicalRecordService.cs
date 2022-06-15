public class MedicalRecordService : IMedicalRecordService
{
    private IMedicalRecordRepository _medicalRecordRepository;

    private IMedicalCardRepository _medicalCardRepository;

    private IDrugRepository _drugRepository;

    public MedicalRecordService()
    {
        _medicalRecordRepository = new MedicalRecordRepository();
        _medicalCardRepository = new MedicalCardRepository();
        _drugRepository = new DrugRepository();
    }

    public async Task UpdateMedicalRecord(int id, MedicalRecord medicalRecord)
    {
        await _medicalRecordRepository.UpdateMedicalRecord(id, medicalRecord);
    }

    public async Task UpdatePatientsPerscriptionList(int id, Prescription prescription)
    {
        await _medicalRecordRepository.UpdatePatientsPerscriptionList(id, prescription);
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
        return await _medicalRecordRepository.GetPrescriptions(id);
    }
    public async Task<Prescription> GetPrescription(string drug, int id)
    {
        return await _medicalRecordRepository.GetPrescription(drug, id);
    }
}