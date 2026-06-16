using DAMS.API.DTOs;
using DAMS.Core.Enums;

namespace DAMS.API.Services;

public interface IAppointmentService
{
    Task<List<AppointmentDto>> GetAllAsync(AppointmentStatus? status);
    Task<List<AppointmentDto>> GetForDoctorUserAsync(int userId);
    Task<List<AppointmentDto>> GetForPatientUserAsync(int userId);
    Task<int?> ResolvePatientIdForUserAsync(int userId);

    Task<BookingResult> CreateAsync(int patientId, CreateAppointmentRequest request);
    Task<AppointmentDto?> UpdateStatusAsync(int id, AppointmentStatus status);
}
