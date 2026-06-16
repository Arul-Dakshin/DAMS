using System.ComponentModel.DataAnnotations;

namespace DAMS.API.DTOs;

public record PrescriptionItemDto(
    int Id,
    string MedicineName,
    string Dosage,
    string Frequency,
    int DurationDays,
    string? Instructions);

public record PrescriptionDto(
    int Id,
    int AppointmentId,
    int DoctorId,
    string DoctorName,
    int PatientId,
    string PatientName,
    string Diagnosis,
    string? Notes,
    DateTime CreatedAt,
    DateTime AppointmentDate,
    List<PrescriptionItemDto> Items);

public record CreatePrescriptionItem(
    [Required, MaxLength(150)] string MedicineName,
    [Required, MaxLength(50)] string Dosage,
    [Required, MaxLength(50)] string Frequency,
    [Range(1, 365)] int DurationDays,
    [MaxLength(200)] string? Instructions);

public record CreatePrescriptionRequest(
    [Required] int AppointmentId,
    [Required, MaxLength(1000)] string Diagnosis,
    [MaxLength(1000)] string? Notes,
    [MinLength(1)] List<CreatePrescriptionItem> Items);

public record PrescriptionResult(PrescriptionDto? Prescription, string? Error = null, bool IsConflict = false);
