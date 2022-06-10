public interface IExaminationService
{
    public Task<List<Examination>> GetAllExaminations();

    public Task<List<Examination>> GetAllDoctorsExaminations(int doctorId);

    public Task<List<Examination>> GetDoctorsExaminationsSchedule(int doctorId, string dateOfSearch);

    public Task<List<Examination>> GetAllPatientsExaminations(int patientId);

    public Task SaveExamination(Examination examination);

    public Task UpdateExamination(int id, Examination examination);

    public Task DeleteExamination(int id);

    public Task<bool> IsNewExaminationValid(Examination examination);

    public Task<bool> IsUpdatedExaminationValid(Examination examination);

    public Task<bool> IsRoomOccupied(string examinationRoomName, string dateAndTimeOfExamination, int durationOfExamination);

    public Task<bool> IsValidPatient(int id);

    public Task<bool> IsRoomInRenovation(string roomName, string examinationDate);

    public Task<bool> IsPatientFree(int id, string dateAndTimeOfExamination, int durationOfExamination, int examinationId);

    public Task<bool> IsDoctorFree(int id, string dateAndTimeOfExamination, int durationOfExamination, int examinationId);

    public Task<bool> ExaminationScheduledAtThatTime(Renovation renovation);
}