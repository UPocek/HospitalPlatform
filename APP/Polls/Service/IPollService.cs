public interface IPollService
{
    public Task<Hospital> GetHospitalPolls();

    public Task<List<PollForDoctors>> GetDoctorPolls();

    public Task PostPoll(Poll poll);
}