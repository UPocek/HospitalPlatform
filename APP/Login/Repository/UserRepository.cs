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

    public IActionResult GetAllDoctors()
    {
        var collection = _database.GetCollection<BsonDocument>("Employees");
        var filter = Builders<BsonDocument>.Filter.Eq("role", "doctor");
        var result = collection.Find(filter).ToList();
        var dotNetObjList = result.ConvertAll(BsonTypeMapper.MapToDotNetValue);
        return new JsonResult(dotNetObjList);
;   }

    public IActionResult GetEmployee(int id)
    {
        var collection = _database.GetCollection<BsonDocument>("Employees");
        var filter = Builders<BsonDocument>.Filter.Eq("id", id);
        var result = collection.Find(filter).FirstOrDefault();
        var wantedUser = BsonTypeMapper.MapToDotNetValue(result);
        return new JsonResult(wantedUser);
    }

    public async Task<List<Employee>> GetDoctors(){
         var collection = _database.GetCollection<Employee>("Employees");

        return await collection.Find(e => e.Role == "doctor").ToListAsync();
    }
    public async Task<Employee> GetDoctor(int doctorId)
    {
        var collection = _database.GetCollection<Employee>("Employees");

        return await collection.Find(e => e.Role == "doctor" && e.Id == doctorId).FirstOrDefaultAsync();
    }
    public async Task UpdateDoctor(int doctorId, Employee doctor){
        var collection = _database.GetCollection<Employee>("Employees");

        await collection.ReplaceOneAsync(doctor => doctor.Id == doctorId, doctor);
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

    public async Task<List<Employee>> GetSpecializedDoctors(string specialization){
        var collectionDoctors = _database.GetCollection<Employee>("Employees");
        return await collectionDoctors.Find(e => e.Role == "doctor" && e.Specialization == specialization).ToListAsync();

    }

    public async Task<List<String>> GetDoctorSpecializations()
    {
        var collection = _database.GetCollection<Employee>("Employees");
        var doctors = await collection.Find(d => d.Role == "doctor").ToListAsync();

        List<string> allSpecializations = new List<string>();

        foreach (Employee e in doctors)
        {
            allSpecializations.Add(e.Specialization);
        }

        allSpecializations = allSpecializations.Distinct().ToList();

        return allSpecializations;
    }

    

}