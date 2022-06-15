public interface IRenovationRepository
{
    public Task<List<Renovation>> GetAllRenovations();

    public Task<List<Renovation>> GetAllRenovationsInRoom(string roomName);

    public Task InsertRenovation(Renovation renovation);

    public Task StartRenovationInRoom(string roomName);
}