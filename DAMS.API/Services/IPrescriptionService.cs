using DAMS.API.DTOs;

namespace DAMS.API.Services;

public interface IPrescriptionService
{
    Task<PrescriptionResult> CreateAsync(int userId, CreatePrescriptionRequest request);
    Task<List<PrescriptionDto>> GetForUserAsync(string role, int userId);
    Task<PrescriptionDto?> GetByIdAsync(int id, string role, int userId);
    Task<PrescriptionDto?> GetByAppointmentAsync(int appointmentId);
}
