public interface IExaminationRepository
{
    public Task<List<Examination>> GetAllExaminations();

    public Task<Examination> GetExamination(int id);

    public Task<List<Examination>> GetAllDoctorsExaminations(int doctorId);

    public Task<List<Examination>> GetAllPatientsExaminations(int patientId);

    public Task DeleteExamination(int id);

    public Task InsertExamination(Examination examination);

    public Task UpdateExamination(int id, Examination examination);

    public Task<List<Examination>> GetAllExaminationsInRoom(string roomName);

    public Task<List<Examination>> GetExaminationsAfterNow(Examination examination);
}