public interface IRenovationService
{
    public Task<bool> RenovationScheduledAtThatTime(Renovation renovation);

    public Task SaveRenovation(Renovation renovation);

    public Task StartSimpleRenovation(Renovation renovation);

    public Task StartDevideRenovation(Renovation renovation);

    public Task StartMergeRenovation(Renovation renovation);

    public Task<List<Renovation>> GetAllRenovations();
}