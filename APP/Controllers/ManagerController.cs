#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APP.Models;
using MongoDB.Bson;
using MongoDB.Driver;

[ApiController]
[Route("api/[controller]")]
public class ManagerController : ControllerBase
{

    private IMongoDatabase database;
    public ManagerController()
    {
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
        var client = new MongoClient(settings);
        database = client.GetDatabase("USI");
    }

    // GET: api/Manager/rooms
    [HttpGet("rooms")]
    public IActionResult GetUser(int id)
    {
        IMongoCollection<BsonDocument> collection;

        collection = database.GetCollection<BsonDocument>("Rooms");

        var filter = Builders<BsonDocument>.Filter.Empty;
        var results = collection.Find(filter).ToList();
        var dotNetObjList = results.ConvertAll(BsonTypeMapper.MapToDotNetValue);
        Response.StatusCode = StatusCodes.Status200OK;
        return new JsonResult(dotNetObjList);
    }

    // GET by Id action

    // POST action

    // PUT action

    // DELETE action
}
