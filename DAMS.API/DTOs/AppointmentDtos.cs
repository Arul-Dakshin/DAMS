using System.ComponentModel.DataAnnotations;
using DAMS.Core.Enums;

namespace DAMS.API.DTOs;

public record AppointmentDto(
    int Id,
    int PatientId,
    string PatientName,
    int DoctorId,
    string DoctorName,
    string Specialization,
    DateTime ScheduledStart,
    DateTime ScheduledEnd,
    AppointmentStatus Status,
    AppointmentType Type,
    string? Reason);

public record CreateAppointmentRequest(
    int PatientId,
    [Required] int DoctorId,
    [Required] DateTime ScheduledStart,
    AppointmentType Type = AppointmentType.OPD,
    [MaxLength(500)] string? Reason = null);

public record UpdateAppointmentStatusRequest([Required] AppointmentStatus Status);

// Result wrapper so the controller can map domain errors to 400 vs 409.
public record BookingResult(AppointmentDto? Appointment, string? Error = null, bool IsConflict = false);
