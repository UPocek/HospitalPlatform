public class ExaminationRequestService : IExaminationRequestService
{
    private IExaminationRequestRepository examinationRequestRepository;

    public ExaminationRequestService()
    {
        examinationRequestRepository = new ExaminationRequestRepository();
    }

    public async Task<List<ExaminationRequest>> GetExaminationRequests()
    {
        return await examinationRequestRepository.GetExaminationRequests();
    }

    public async Task AcceptExaminationRequest(string id)
    {
        await examinationRequestRepository.AcceptExaminationRequest(id);
    }


    public async Task DeclineExaminationRequest(string id)
    {
        await examinationRequestRepository.DeclineExaminationRequest(id);
    }
    
    public async Task CreateRequest(ExaminationRequest request){
        await examinationRequestRepository.CreateRequest(request);
    }

}