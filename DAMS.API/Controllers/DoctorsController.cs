using DAMS.API.Authorization;
using DAMS.API.DTOs;
using DAMS.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DAMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _service;

    public DoctorsController(IDoctorService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<DoctorDto>>> GetAll([FromQuery] string? search)
        => Ok(await _service.GetAllAsync(search));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DoctorDto>> Get(int id)
    {
        var doctor = await _service.GetByIdAsync(id);
        return doctor is null ? NotFound() : Ok(doctor);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<DoctorDto>> Create(CreateDoctorRequest request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<DoctorDto>> Update(int id, UpdateDoctorRequest request)
    {
        var updated = await _service.UpdateAsync(id, request);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id)
        => await _service.DeleteAsync(id) ? NoContent() : NotFound();

    [HttpGet("{id:int}/schedules")]
    public async Task<ActionResult<List<DoctorScheduleDto>>> GetSchedules(int id)
        => Ok(await _service.GetSchedulesAsync(id));

    [HttpPost("{id:int}/schedules")]
    [Authorize(Roles = Roles.Admin + "," + Roles.Doctor)]
    public async Task<ActionResult<DoctorScheduleDto>> AddSchedule(int id, CreateScheduleRequest request)
    {
        var created = await _service.AddScheduleAsync(id, request);
        return created is null
            ? BadRequest(new { message = "Doctor not found or end time must be after start time." })
            : Ok(created);
    }

    [HttpDelete("{id:int}/schedules/{scheduleId:int}")]
    [Authorize(Roles = Roles.Admin + "," + Roles.Doctor)]
    public async Task<IActionResult> DeleteSchedule(int id, int scheduleId)
        => await _service.DeleteScheduleAsync(id, scheduleId) ? NoContent() : NotFound();

    [HttpGet("{id:int}/slots")]
    public async Task<ActionResult<List<SlotDto>>> GetSlots(int id, [FromQuery] DateOnly date)
        => Ok(await _service.GetAvailableSlotsAsync(id, date));
}
