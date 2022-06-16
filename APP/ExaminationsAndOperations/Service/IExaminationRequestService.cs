public interface IExaminationRequestService
{
    public Task<List<ExaminationRequest>> GetExaminationRequests();
    public Task AcceptExaminationRequest(string id);
    public Task DeclineExaminationRequest(string id);

    public Task CreateRequest(ExaminationRequest request);
}