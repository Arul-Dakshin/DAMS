using System.Security.Claims;
using DAMS.API.Authorization;
using DAMS.API.DTOs;
using DAMS.API.Services;
using DAMS.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DAMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _service;

    public AppointmentsController(IAppointmentService service) => _service = service;

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<List<AppointmentDto>>> GetAll([FromQuery] AppointmentStatus? status)
    {
        if (User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Receptionist))
            return Ok(await _service.GetAllAsync(status));
        if (User.IsInRole(Roles.Doctor))
            return Ok(await _service.GetForDoctorUserAsync(CurrentUserId));
        return Ok(await _service.GetForPatientUserAsync(CurrentUserId));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin + "," + Roles.Receptionist + "," + Roles.Patient)]
    public async Task<ActionResult<AppointmentDto>> Book(CreateAppointmentRequest request)
    {
        int patientId;
        if (User.IsInRole(Roles.Patient))
        {
            var resolved = await _service.ResolvePatientIdForUserAsync(CurrentUserId);
            if (resolved is null)
                return BadRequest(new { message = "No patient profile is linked to your account yet." });
            patientId = resolved.Value;
        }
        else
        {
            if (request.PatientId <= 0)
                return BadRequest(new { message = "A patient must be selected." });
            patientId = request.PatientId;
        }

        var result = await _service.CreateAsync(patientId, request);
        if (result.Error is not null)
            return result.IsConflict
                ? Conflict(new { message = result.Error })
                : BadRequest(new { message = result.Error });

        return CreatedAtAction(nameof(GetAll), new { id = result.Appointment!.Id }, result.Appointment);
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = Roles.Staff)]
    public async Task<ActionResult<AppointmentDto>> UpdateStatus(int id, UpdateAppointmentStatusRequest request)
    {
        var updated = await _service.UpdateStatusAsync(id, request.Status);
        return updated is null ? NotFound() : Ok(updated);
    }
}
