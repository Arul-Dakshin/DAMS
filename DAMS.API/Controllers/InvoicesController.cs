using System.Security.Claims;
using DAMS.API.Authorization;
using DAMS.API.DTOs;
using DAMS.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DAMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin + "," + Roles.Receptionist + "," + Roles.Patient)]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _service;

    public InvoicesController(IInvoiceService service) => _service = service;

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private string CurrentRole => User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    [HttpGet]
    public async Task<ActionResult<List<InvoiceDto>>> GetAll()
        => Ok(await _service.GetForUserAsync(CurrentRole, CurrentUserId));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<InvoiceDto>> Get(int id)
    {
        var dto = await _service.GetByIdAsync(id, CurrentRole, CurrentUserId);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = Roles.FrontDesk)]
    public async Task<ActionResult<InvoiceDto>> Create(CreateInvoiceRequest request)
    {
        var dto = await _service.CreateAsync(request);
        return dto is null
            ? BadRequest(new { message = "Patient not found." })
            : CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPost("from-appointment/{appointmentId:int}")]
    [Authorize(Roles = Roles.FrontDesk)]
    public async Task<ActionResult<InvoiceDto>> CreateFromAppointment(int appointmentId)
    {
        var dto = await _service.CreateFromAppointmentAsync(appointmentId);
        return dto is null
            ? BadRequest(new { message = "Appointment not found." })
            : CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}/pay")]
    [Authorize(Roles = Roles.FrontDesk)]
    public async Task<ActionResult<InvoiceDto>> MarkPaid(int id)
    {
        var dto = await _service.MarkPaidAsync(id);
        return dto is null
            ? BadRequest(new { message = "Invoice not found or already paid." })
            : Ok(dto);
    }
}
