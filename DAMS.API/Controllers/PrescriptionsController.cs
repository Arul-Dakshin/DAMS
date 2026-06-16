using System.Security.Claims;
using DAMS.API.Authorization;
using DAMS.API.DTOs;
using DAMS.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DAMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PrescriptionsController : ControllerBase
{
    private readonly IPrescriptionService _service;

    public PrescriptionsController(IPrescriptionService service) => _service = service;

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private string CurrentRole => User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    [HttpGet]
    public async Task<ActionResult<List<PrescriptionDto>>> GetAll()
        => Ok(await _service.GetForUserAsync(CurrentRole, CurrentUserId));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PrescriptionDto>> Get(int id)
    {
        var dto = await _service.GetByIdAsync(id, CurrentRole, CurrentUserId);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet("by-appointment/{appointmentId:int}")]
    [Authorize(Roles = Roles.Staff)]
    public async Task<ActionResult<PrescriptionDto>> GetByAppointment(int appointmentId)
    {
        var dto = await _service.GetByAppointmentAsync(appointmentId);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Doctor)]
    public async Task<ActionResult<PrescriptionDto>> Create(CreatePrescriptionRequest request)
    {
        var result = await _service.CreateAsync(CurrentUserId, request);
        if (result.Error is not null)
            return result.IsConflict
                ? Conflict(new { message = result.Error })
                : BadRequest(new { message = result.Error });
        return CreatedAtAction(nameof(Get), new { id = result.Prescription!.Id }, result.Prescription);
    }
}
