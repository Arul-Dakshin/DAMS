using DAMS.API.DTOs;

namespace DAMS.API.Services;

public interface IDoctorService
{
    Task<List<DoctorDto>> GetAllAsync(string? search);
    Task<DoctorDto?> GetByIdAsync(int id);
    Task<DoctorDto> CreateAsync(CreateDoctorRequest request);
    Task<DoctorDto?> UpdateAsync(int id, UpdateDoctorRequest request);
    Task<bool> DeleteAsync(int id);

    Task<List<DoctorScheduleDto>> GetSchedulesAsync(int doctorId);
    Task<DoctorScheduleDto?> AddScheduleAsync(int doctorId, CreateScheduleRequest request);
    Task<bool> DeleteScheduleAsync(int doctorId, int scheduleId);

    Task<List<SlotDto>> GetAvailableSlotsAsync(int doctorId, DateOnly date);
}
