public interface IValidateExaminationService
{
    public Task<bool> IsExaminationValid(Examination examination);
    public Task<bool> IsNewExaminationValid(Examination examination);
    public Task<bool> IsUpdatedExaminationValid(Examination examination);
    public Task<bool> IsRoomOccupied(string examinationRoomName, string dateAndTimeOfExamination, int durationOfExamination);
    public Task<bool> IsRoomOccupiedForUpdate(string examinationRoomName, string dateAndTimeOfExamination, int durationOfExamination, int examinationId);
    public Task<bool> IsValidPatient(int id);
    public Task<bool> IsRoomValid(string roomName);
    public Task<bool> IsRoomInRenovation(string roomName, string examinationDate);
    public Task<bool> IsPatientFree(int id, string dateAndTimeOfExamination, int durationOfExamination, int examinationId);
    public Task<bool> IsDoctorFree(int id, string dateAndTimeOfExamination, int durationOfExamination, int examinationId);
    public bool IsTimeInBetween(string dateAndTimeOfExamination, int durationOfExamination, Examination examination);
    public bool IsDateInBetween(string startDate, string endDate, string examinationDate);
}