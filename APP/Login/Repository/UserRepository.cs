using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

public class UserRepository : IUserRepository
{
    private readonly IMongoDatabase _database;

    public UserRepository()
    {
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        _database = client.GetDatabase("USI");
    }

    public async Task<List<Employee>> GetAllDoctors()
    {
        var collection = _database.GetCollection<Employee>("Employees");
        return await collection.Find(e => e.Role == "doctor").ToListAsync();
    }

    public IActionResult GetEmployee(int id)
    {
        var collection = _database.GetCollection<BsonDocument>("Employees");
        var filter = Builders<BsonDocument>.Filter.Eq("id", id);
        var result = collection.Find(filter).FirstOrDefault();
        var wantedUser = BsonTypeMapper.MapToDotNetValue(result);
        return new JsonResult(wantedUser);
    }

    public async Task<Employee> GetDoctor(int doctorId)
    {
        var collection = _database.GetCollection<Employee>("Employees");

        return await collection.Find(e => e.Role == "doctor" && e.Id == doctorId).FirstOrDefaultAsync();
    }

    public IActionResult GetPatient(int patientId)
    {
        var collection = _database.GetCollection<BsonDocument>("Patients");
        var filter = Builders<BsonDocument>.Filter.Eq("id", patientId);
        var result = collection.Find(filter).FirstOrDefault();
        var wantedUser = BsonTypeMapper.MapToDotNetValue(result);
        return new JsonResult(wantedUser);
    }

    public async Task<User> LoginEmployee(string email, string password)
    {
        var collection = _database.GetCollection<User>("Employees");
        return await collection.Find(item => item.Email == email & item.Password == password).FirstOrDefaultAsync();
    }

    public async Task<User> LoginPatient(string email, string password)
    {
        var collectionPatients = _database.GetCollection<User>("Patients");
        return await collectionPatients.Find(item => item.Email == email & item.Password == password).FirstOrDefaultAsync();
    }

}