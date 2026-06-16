using System.ComponentModel.DataAnnotations;
using DAMS.Core.Enums;

namespace DAMS.API.DTOs;

public record InvoiceItemDto(int Id, string Description, int Quantity, decimal UnitPrice, decimal Amount);

public record InvoiceDto(
    int Id,
    string InvoiceNumber,
    int PatientId,
    string PatientName,
    int? AppointmentId,
    DateTime IssuedDate,
    InvoiceStatus Status,
    DateTime? PaidDate,
    string? Notes,
    decimal Total,
    List<InvoiceItemDto> Items);

public record CreateInvoiceItem(
    [Required, MaxLength(200)] string Description,
    [Range(1, 1000)] int Quantity,
    [Range(0, 1000000)] decimal UnitPrice);

public record CreateInvoiceRequest(
    [Required] int PatientId,
    [MaxLength(500)] string? Notes,
    [MinLength(1)] List<CreateInvoiceItem> Items);
