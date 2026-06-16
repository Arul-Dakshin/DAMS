using DAMS.API.DTOs;

namespace DAMS.API.Services;

public interface IPatientService
{
    Task<List<PatientDto>> GetAllAsync(string? search);
    Task<PatientDto?> GetByIdAsync(int id);
    Task<PatientDto?> GetByUserIdAsync(int userId);
    Task<PatientDto> CreateAsync(CreatePatientRequest request);
    Task<PatientDto?> UpdateAsync(int id, UpdatePatientRequest request);
    Task<bool> DeleteAsync(int id);
}
