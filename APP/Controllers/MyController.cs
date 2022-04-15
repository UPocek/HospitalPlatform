#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APP.Models;

namespace APP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyController : ControllerBase
    {
        private readonly MyContext _context;

        public MyController(MyContext context)
        {
            _context = context;
        }

        // HOW TO : CheetSheet

        // Run app - dotnet watch
        // Stop app - Ctrl + c

        // GET: api/My
        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<Other>>> GetOthers()
        // {
        //     return await _context.Others.ToListAsync();
        // }

        // GET: api/My
        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<Other>>> GetOthers()
        // {
        //     return BadRequest();
        //     return NotFound();
        //     return Ok();
        // }

        // https://www.tutorialsteacher.com/csharp/csharp-dictionary
        // https://www.telerik.com/blogs/return-json-result-custom-status-code-aspnet-core#altering-the-response-status-code
        // https://developer.mozilla.org/en-US/docs/Web/HTTP/Status
        // [HttpGet]
        // public IActionResult Get()
        // {
        //     var result = new Dictionary<string, string>();
        //     result["A"] = "a";
        //     result["B"] = "b";
        //     Response.StatusCode = StatusCodes.Status200OK;

        //     return new JsonResult(result);
        // }

        // // GET: api/My/5
        // [HttpGet("{id}")]
        // Use id? to specify that id is optional
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
