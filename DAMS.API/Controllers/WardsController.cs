using DAMS.API.Authorization;
using DAMS.API.DTOs;
using DAMS.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DAMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Staff)]
public class WardsController : ControllerBase
{
    private readonly IWardService _service;

    public WardsController(IWardService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<WardDto>>> GetAll()
        => Ok(await _service.GetWardsAsync());

    [HttpGet("available-beds")]
    public async Task<ActionResult<List<BedDto>>> GetAvailableBeds()
        => Ok(await _service.GetAvailableBedsAsync());

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<WardDto>> Create(CreateWardRequest request)
    {
        var ward = await _service.CreateWardAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = ward.Id }, ward);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id)
        => await _service.DeleteWardAsync(id) ? NoContent() : NotFound();

    [HttpPost("{wardId:int}/beds")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<BedDto>> AddBed(int wardId, CreateBedRequest request)
    {
        var bed = await _service.AddBedAsync(wardId, request);
        return bed is null
            ? BadRequest(new { message = "Ward not found or that bed number already exists in the ward." })
            : Ok(bed);
    }

    [HttpDelete("{wardId:int}/beds/{bedId:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteBed(int wardId, int bedId)
        => await _service.DeleteBedAsync(wardId, bedId)
            ? NoContent()
            : BadRequest(new { message = "Bed not found or it is currently occupied." });

    [HttpPut("beds/{bedId:int}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<BedDto>> UpdateBedStatus(int bedId, UpdateBedStatusRequest request)
    {
        var bed = await _service.UpdateBedStatusAsync(bedId, request);
        return bed is null
            ? BadRequest(new { message = "Bed not found, or occupied beds can only change status via discharge." })
            : Ok(bed);
    }
}
