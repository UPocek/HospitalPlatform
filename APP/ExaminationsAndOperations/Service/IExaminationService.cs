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

    public Task<bool> IsPatientFree(int id, string dateAndTimeOfExamination, int durationOfExamination);

    public Task<bool> IsDoctorFree(int id, string dateAndTimeOfExamination, int durationOfExamination);

    public Task<bool> ExaminationScheduledAtThatTime(Renovation renovation);

    public bool isDurationValid(Examination newExamination);

    public Task<Examination> GetExamination(int id);

    public Task<bool> doesSpecializedDoctorExist(int doctorId,string specialization);

    public Task<bool> doesRoomExist(string roomName);

    public Task CreateRefferedExamination(Examination newExamination, string specialization, int referralid);

    public Task CreateUrgentExaminationWithTermMoving(Examination examination);

    public Task<List<Examination>> CreateUrgentExamination(Examination examination, string specialization);

    public void SendTermNotificationEmailToPatient(Patient patient, Employee employee, string oldDateAndTime, string newDateAndTime, int? examId);

    public void SendTermNotificationEmailToDoctor(Patient patient, Employee employee, string oldDateAndTime, string newDateAndTime, int? examId);

    public bool TrollCheck(Patient patient, String type, int n);
}