public class DoctorService : IDoctorService
{
    private IManagerRepository repository;

    public DoctorService()
    {
        repository = new ManagerRepository();
    }
}