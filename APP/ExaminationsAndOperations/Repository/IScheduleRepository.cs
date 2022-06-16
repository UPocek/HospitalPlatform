public interface IScheduleRepository
{
    public Task<List<Examination>> GetAllExaminations();

    public Task<Examination> GetExamination(int id);

    public Task<List<Examination>> GetAllDoctorsExaminations(int doctorId);

    public Task<List<Examination>> GetAllPatientsExaminations(int patientId);

    public Task<List<Examination>> GetAvailableExamination(ExaminationFilter filter);


    public Task DeleteExamination(int id);

    public Task DeletePatientsExamination(int id);


    public Task InsertExamination(Examination examination);

    public Task CreateExamination(Examination examination);

    public Task UpdateExamination(int id, Examination examination);
    
    public Task UpdatePatientsExamination(string id, Examination examination);

    public Task<List<Examination>> GetAllExaminationsInRoom(string roomName);

    public Task<List<Examination>> GetExaminationsAfterNow(Examination examination);

      

}