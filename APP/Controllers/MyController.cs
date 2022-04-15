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

namespace APP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyController : ControllerBase
    {
        private readonly MyContext _context;
        public IMongoDatabase database;

        public MyController(MyContext context)
        {
            _context = context;


            var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
            var client = new MongoClient(settings);
            database = client.GetDatabase("USI");

        }

        // GET: api/My/login/u&p
        [HttpGet("login/{email}&{password}")]
        public IActionResult UserLogin(string email, string password)
        {
            var collection = database.GetCollection<BsonDocument>("Employees");
            var filter = Builders<BsonDocument>.Filter.Eq("email", email) & Builders<BsonDocument>.Filter.Eq("password", password);
            var doc = collection.Find(filter).ToList();
            // var dotNetObjList = bsonDocList.ConvertAll(BsonTypeMapper.MapToDotNetValue);
            if (doc.Count() != 0)
            {
                var dotNetObj = BsonTypeMapper.MapToDotNetValue(doc[0]);
                Response.StatusCode = StatusCodes.Status200OK;
                return new JsonResult(dotNetObj);
            }
            else
            {
                collection = database.GetCollection<BsonDocument>("Patients");
                doc = collection.Find(filter).ToList();
                var dotNetObj = BsonTypeMapper.MapToDotNetValue(doc[0]);
                if (doc.Count() != 0)
                {
                    Response.StatusCode = StatusCodes.Status200OK;
                    return new JsonResult(dotNetObj);
                }
                return NotFound();
            }
        }

        // HOW TO : CheetSheet

        // Run app - dotnet watch
        // Stop app - Ctrl + c

        // // GET: api/My
        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<Other>>> GetOthers()
        // {
        //     return await _context.Others.ToListAsync();
        // }

        // // GET: api/My
        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<Other>>> GetOthers()
        // {
        //     return BadRequest();
        //     return NotFound();
        //     return Ok();
        // }

        // // https://www.tutorialsteacher.com/csharp/csharp-dictionary
        // // https://www.telerik.com/blogs/return-json-result-custom-status-code-aspnet-core#altering-the-response-status-code
        // // https://developer.mozilla.org/en-US/docs/Web/HTTP/Status
        // [HttpGet]
        // public IActionResult Get()
        // {
        //     var result = new Dictionary<string, string>();
        //     result["A"] = "a";
        //     result["B"] = "b";
        //     Response.StatusCode = StatusCodes.Status200OK;

        //     return new JsonResult(result);
        // }

        // // GET: api/My
        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<Other>>> GetOthers()
        // {
        //     // Connect to collection you want
        //     var collection = database.GetCollection<BsonDocument>("My");

        //     // 0 - Create
        //     var document = new BsonDocument
        //     {
        //         { "student_id", 10000 },
        //         { "scores", new BsonArray
        //             {
        //             new BsonDocument{ {"type", "exam"}, {"score", 88.12334193287023 } },
        //             new BsonDocument{ {"type", "quiz"}, {"score", 74.92381029342834 } },
        //             new BsonDocument{ {"type", "homework"}, {"score", 89.97929384290324 } },
        //             new BsonDocument{ {"type", "homework"}, {"score", 82.12931030513218 } }
        //             }
        //         },
        //         { "class_id", 480}
        //     };

        //     collection.InsertOne(document);
        //     // await collection.InsertOneAsync(document);


        //     // 1 - Get first result that satisfies filter
        //     var filter = Builders<BsonDocument>.Filter.Eq("test", "test") & Builders<BsonDocument>.Filter.Eq("test", "test");
        //     try
        //     {
        //         var doc = collection.Find(filter).FirstOrDefault();
        //         Console.WriteLine(doc.ToString());
        //     }
        //     catch
        //     {
        //         Console.WriteLine("Filter nije vratio ni jedan element!");
        //     }

        //     // 2 - Get all results without filter
        //     var documents = collection.Find(new BsonDocument()).ToList();

        //     foreach (BsonDocument doc in documents)
        //     {
        //         Console.WriteLine(doc.ToString());
        //     }

        //     // 3 - Query
        //     var builder = Builders<BsonDocument>.Filter;
        //     var filter2 = builder.Gt("test", 0) & builder.Lt("test", 100);
        //     var sort = Builders<BsonDocument>.Sort.Descending("test");

        //     var docs = collection.Find(filter2).Project("{_id: 1}").Sort(sort).Skip(0).Limit(3).ToList();

        //     docs.ForEach(doc =>
        //     {
        //         Console.WriteLine(doc);
        //     });

        //     // 4 - Delete

        //     collection.DeleteOne(filter);

        //     // 5 - Update

        //     var filter3 = Builders<BsonDocument>.Filter.Eq("name", "Audi");
        //     var update = Builders<BsonDocument>.Update.Set("price", 52000);
        //     collection.UpdateOne(filter3, update);

        //     return await _context.Others.ToListAsync();
        // }

        // // GET: api/My/5
        // [HttpGet("{id}")]
        // // Use id? to specify that id is optional
        // public async Task<ActionResult<Other>> GetOther(long id)
        // {
        //     Console.WriteLine(id);
        //     var other = await _context.Others.FindAsync(id);

        //     if (other == null)
        //     {
        //         return NotFound();
        //     }

        //     return other;
        // }

        // // PUT: api/My/5
        // // https://developer.mozilla.org/en-US/docs/Web/API/XMLHttpRequest/send
        // [HttpPut("{id}")]
        // public async Task<IActionResult> PutOther(long id, Other other)
        // {
        //     if (id != other.Id)
        //     {
        //         return BadRequest();
        //     }

        //     _context.Entry(other).State = EntityState.Modified;

        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         if (!OtherExists(id))
        //         {
        //             return NotFound();
        //         }
        //         else
        //         {
        //             throw;
        //         }
        //     }

        //     return NoContent();
        // }

        // // POST: api/My
        // [HttpPost]
        // public async Task<ActionResult<Other>> PostOther(Other other)
        // {
        //     _context.Others.Add(other);
        //     await _context.SaveChangesAsync();

        //     return CreatedAtAction(nameof(GetOther), new { id = other.Id }, other);
        // }

        // // DELETE: api/My/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteOther(long id)
        // {
        //     var other = await _context.Others.FindAsync(id);
        //     if (other == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.Others.Remove(other);
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }

        // private bool OtherExists(long id)
        // {
        //     return _context.Others.Any(e => e.Id == id);
        // }
    }
}
