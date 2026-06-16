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
public class PatientsController : ControllerBase
{
    private readonly IPatientService _service;

    public PatientsController(IPatientService service) => _service = service;

    [HttpGet]
    [Authorize(Roles = Roles.Staff)]
    public async Task<ActionResult<List<PatientDto>>> GetAll([FromQuery] string? search)
        => Ok(await _service.GetAllAsync(search));

    [HttpGet("{id:int}")]
    [Authorize(Roles = Roles.Staff)]
    public async Task<ActionResult<PatientDto>> Get(int id)
    {
        var patient = await _service.GetByIdAsync(id);
        return patient is null ? NotFound() : Ok(patient);
    }

    [HttpGet("me")]
    [Authorize(Roles = Roles.Patient)]
    public async Task<ActionResult<PatientDto>> Me()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var patient = await _service.GetByUserIdAsync(userId);
        return patient is null
            ? NotFound(new { message = "No patient profile is linked to your account yet." })
            : Ok(patient);
    }

    [HttpPost]
    [Authorize(Roles = Roles.FrontDesk)]
    public async Task<ActionResult<PatientDto>> Create(CreatePatientRequest request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.FrontDesk)]
    public async Task<ActionResult<PatientDto>> Update(int id, UpdatePatientRequest request)
    {
        var updated = await _service.UpdateAsync(id, request);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id)
        => await _service.DeleteAsync(id) ? NoContent() : NotFound();
}
