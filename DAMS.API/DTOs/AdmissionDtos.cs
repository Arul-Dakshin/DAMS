using System.ComponentModel.DataAnnotations;
using DAMS.Core.Enums;

namespace DAMS.API.DTOs;

public record AdmissionDto(
    int Id,
    int PatientId,
    string PatientName,
    int BedId,
    string BedNumber,
    string WardName,
    DateTime AdmitDate,
    DateTime? DischargeDate,
    AdmissionStatus Status,
    string? Reason);

public record CreateAdmissionRequest(
    [Required] int PatientId,
    [Required] int BedId,
    [MaxLength(500)] string? Reason);

// Result wrapper so the controller can map domain errors to 400/409.
public record AdmissionResult(AdmissionDto? Admission, string? Error = null, bool IsConflict = false);
