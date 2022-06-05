public interface IExaminationRequestRepository
{
    public Task<List<ExaminationRequest>> GetExaminationRequests();
    public Task AcceptExaminationRequest(string id);
    public Task DeclineExaminationRequest(string id);
}