using DAMS.API.DTOs;

namespace DAMS.API.Services;

public interface IInvoiceService
{
    Task<List<InvoiceDto>> GetForUserAsync(string role, int userId);
    Task<InvoiceDto?> GetByIdAsync(int id, string role, int userId);
    Task<InvoiceDto?> CreateAsync(CreateInvoiceRequest request);
    Task<InvoiceDto?> CreateFromAppointmentAsync(int appointmentId);
    Task<InvoiceDto?> MarkPaidAsync(int id);
}
