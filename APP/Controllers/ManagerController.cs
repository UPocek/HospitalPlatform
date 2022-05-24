#nullable disable
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ManagerController : ControllerBase
{
    private IManagerService service;
    private string dateToday;

    public ManagerController()
    {
        service = new ManagerService();

        dateToday = DateTime.Now.ToString("yyyy-MM-dd");
    }

    // GET: api/Manager/rooms
    [HttpGet("rooms")]
    public async Task<List<Room>> GetRooms()
    {
        return await service.GetAllRooms();
    }

    // GET: api/Manager/drugs
    [HttpGet("drugs")]
    public async Task<List<Drug>> GetDrugs()
    {
        return await service.GetAllDrugs();
    }

    // GET: api/Manager/ingredients
    [HttpGet("ingredients")]
    public async Task<DrugIngredients> GetIngredients()
    {
        return await service.GetAllIngredients();
    }

    // GET: api/Manager/polls
    [HttpGet("polls")]
    public async Task<Hospital> GetHospitalPolls()
    {
        return await service.GetHospitalPolls();
    }

    // GET: api/Manager/doctorpolls
    [HttpGet("doctorpolls")]
    public async Task<List<PollForDoctors>> GetDoctorPolls()
    {
        return await service.GetDoctorPolls();
    }

    // POST: api/Manager/rooms
    [HttpPost("rooms")]
    public async Task<IActionResult> CreateRoom(Room room)
    {
        if (!await service.IsRoomNameValid(room))
        {
            return BadRequest();
        }

        await service.SaveRoom(room);

        return Ok();
    }

    // POST: api/Manager/renovations
    [HttpPost("renovations")]
    public async Task<IActionResult> CreateRenovation(Renovation renovation)
    {
        if (await service.ExaminationScheduledAtThatTime(renovation) || await service.RenovationScheduledAtThatTime(renovation))
        {
            return BadRequest();
        }

        await service.SaveRenovation(renovation);

        if (dateToday == renovation.StartDate)
        {
            await service.StartSimpleRenovation(renovation);
        }

        return Ok();
    }

    // POST: api/Manager/renovationdevide
    [HttpPost("renovationdevide")]
    public async Task<IActionResult> DevideRenovation(Renovation renovation)
    {
        if (await service.ExaminationScheduledAtThatTime(renovation) || await service.RenovationScheduledAtThatTime(renovation))
        {
            return BadRequest();
        }

        if (dateToday == renovation.StartDate)
        {
            await service.StartDevideRenovation(renovation);
        }
        else
        {
            await service.SaveRenovation(renovation);
        }

        return Ok();
    }

    // POST: api/Manager/renovationmerge
    [HttpPost("renovationmerge")]
    public async Task<IActionResult> MergeRenovation(Renovation renovation)
    {
        if (await service.ExaminationScheduledAtThatTime(renovation) || await service.RenovationScheduledAtThatTime(renovation))
        {
            return BadRequest();
        }

        if (dateToday == renovation.StartDate)
        {
            await service.StartMergeRenovation(renovation);
        }
        else
        {
            await service.SaveRenovation(renovation);
        }

        return Ok();
    }

    // POST: api/Manager/transfer
    [HttpPost("transfer")]
    public async Task<IActionResult> CreateTransfer(Transfer transfer)
    {
        if (dateToday == transfer.Date)
        {
            await service.StartTransfer(transfer);
            transfer.Done = true;
        }

        await service.SaveTransfer(transfer);

        return Ok();
    }

    // POST: api/Manager/drugs
    [HttpPost("drugs")]
    public async Task<IActionResult> CreateDrug(Drug drug)
    {
        if (!await service.IsDrugNameValid(drug))
        {
            return BadRequest();
        }

        await service.SaveDrug(drug);

        return Ok();
    }

    // POST: api/Manager/ingredients
    [HttpPost("ingredients")]
    public async Task<IActionResult> CreateIngredinet(Dictionary<string, string> ingredient)
    {
        if (await service.IngredientAlreadyExists(ingredient["name"]))
        {
            return BadRequest();

        }

        await service.SaveIngredients(ingredient["name"]);

        return Ok();
    }


    // PUT: api/Manager/rooms/1
    [HttpPut("rooms/{id}")]
    public async Task<IActionResult> UpdateRoom(string id, Room room)
    {
        if (!await service.IsRoomNameValid(room))
        {
            return BadRequest();
        }

        await service.UpdateRoom(id, room);

        return Ok();
    }

    // PUT: api/Manager/drugs/1
    [HttpPut("drugs/{id}")]
    public async Task<IActionResult> UpdateDrug(string id, Drug drug)
    {
        if (!await service.IsDrugNameValid(drug))
        {
            return BadRequest();
        }

        await service.UpdateDrug(id, drug);

        return Ok();
    }

    // PUT: api/Manager/ingredients
    [HttpPut("ingredients/{id}")]
    public async Task<IActionResult> UpdateIngredinet(string id, Dictionary<string, string> data)
    {
        if (!await service.IngredientAlreadyExists(data["name"]))
        {
            return BadRequest();
        }

        await service.UpdateIngredients(id, data["name"]);

        return Ok();
    }

    // DELETE: api/Manager/rooms/1
    [HttpDelete("rooms/{id}")]
    public async Task<IActionResult> DeleteRoom(string id)
    {
        await service.DeleteRoom(id);

        return Ok();
    }

    // DELETE: api/Manager/drugs/1
    [HttpDelete("drugs/{id}")]
    public async Task<IActionResult> DeleteDrug(string id)
    {
        await service.DeleteDrug(id);

        return Ok();
    }

    // DELETE: api/Manager/ingredients/1
    [HttpDelete("ingredients/{id}")]
    public async Task<IActionResult> DeleteIngredient(string id)
    {
        await service.DeleteIngredient(id);

        return Ok();
    }
}
