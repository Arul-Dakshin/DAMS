using DAMS.API.Authorization;
using DAMS.API.DTOs;
using DAMS.API.Services;
using DAMS.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DAMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Staff)]
public class AdmissionsController : ControllerBase
{
    private readonly IAdmissionService _service;

    public AdmissionsController(IAdmissionService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<AdmissionDto>>> GetAll([FromQuery] AdmissionStatus? status)
        => Ok(await _service.GetAllAsync(status));

    [HttpPost]
    public async Task<ActionResult<AdmissionDto>> Admit(CreateAdmissionRequest request)
    {
        var result = await _service.AdmitAsync(request);
        if (result.Error is not null)
            return result.IsConflict
                ? Conflict(new { message = result.Error })
                : BadRequest(new { message = result.Error });
        return CreatedAtAction(nameof(GetAll), new { id = result.Admission!.Id }, result.Admission);
    }

    [HttpPut("{id:int}/discharge")]
    public async Task<ActionResult<AdmissionDto>> Discharge(int id)
    {
        var discharged = await _service.DischargeAsync(id);
        return discharged is null
            ? BadRequest(new { message = "Admission not found or already discharged." })
            : Ok(discharged);
    }
}
