using System.ComponentModel.DataAnnotations;

namespace DAMS.API.DTOs;

public record DoctorDto(
    int Id,
    string FullName,
    string Specialization,
    string? Phone,
    decimal ConsultationFee,
    int? UserId);

public record CreateDoctorRequest(
    [Required, MaxLength(150)] string FullName,
    [Required, MaxLength(100)] string Specialization,
    [MaxLength(20)] string? Phone,
    [Range(0, 1000000)] decimal ConsultationFee);

public record UpdateDoctorRequest(
    [Required, MaxLength(150)] string FullName,
    [Required, MaxLength(100)] string Specialization,
    [MaxLength(20)] string? Phone,
    [Range(0, 1000000)] decimal ConsultationFee);

public record DoctorScheduleDto(
    int Id,
    int DoctorId,
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int SlotMinutes);

public record CreateScheduleRequest(
    [Required] DayOfWeek DayOfWeek,
    [Required] TimeOnly StartTime,
    [Required] TimeOnly EndTime,
    [Range(5, 240)] int SlotMinutes);

public record SlotDto(DateTime Start, DateTime End);
