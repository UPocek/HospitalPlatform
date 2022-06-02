public class MedicalRecordService : IMedicalRecordService
{
    private IMedicalRecordRepository medicalRecordRepository;

    private IMedicalCardRepository medicalCardRepository;

    private IDrugRepository drugRepository;

    public MedicalRecordService()
    {
        medicalRecordRepository = new MedicalRecordRepository();
        medicalCardRepository = new MedicalCardRepository();
        drugRepository = new DrugRepository();

    }

    public async Task UpdateMedicalRecord(int id, MedicalRecord medicalRecord )
    {
        await medicalRecordRepository.UpdateMedicalRecord(id, medicalRecord);
    }

    public async Task UpdatePatientsPerscriptionList(int id, Prescription prescription )
    {
        await medicalRecordRepository.UpdatePatientsPerscriptionList(id, prescription);
    }

    public async Task<bool> IsPerscriptionValid(int id, Prescription perscription )
    {
        var medicalCard = await medicalCardRepository.GetMedicalCardByPatient(id);
        var drugIngredients = await drugRepository.GetDrugsIngredients(perscription.DrugName);
        foreach(string ingredient in drugIngredients){
            if(medicalCard.MedicalRecord.Alergies.Contains(ingredient)){
                return false;
            }
        }
        return true;

    }
}