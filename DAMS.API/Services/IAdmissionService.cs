using DAMS.API.DTOs;
using DAMS.Core.Enums;

namespace DAMS.API.Services;

public interface IAdmissionService
{
    Task<List<AdmissionDto>> GetAllAsync(AdmissionStatus? status);
    Task<AdmissionResult> AdmitAsync(CreateAdmissionRequest request);
    Task<AdmissionDto?> DischargeAsync(int admissionId);
}
