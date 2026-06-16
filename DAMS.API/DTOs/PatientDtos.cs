using System.ComponentModel.DataAnnotations;
using DAMS.Core.Enums;

namespace DAMS.API.DTOs;

public record PatientDto(
    int Id,
    string FullName,
    Gender Gender,
    DateOnly DateOfBirth,
    string Phone,
    string? Address,
    string? BloodGroup,
    int? UserId,
    DateTime CreatedAt);

public record CreatePatientRequest(
    [Required, MaxLength(150)] string FullName,
    [Required] Gender Gender,
    [Required] DateOnly DateOfBirth,
    [Required, MaxLength(20)] string Phone,
    [MaxLength(300)] string? Address,
    [MaxLength(5)] string? BloodGroup);

public record UpdatePatientRequest(
    [Required, MaxLength(150)] string FullName,
    [Required] Gender Gender,
    [Required] DateOnly DateOfBirth,
    [Required, MaxLength(20)] string Phone,
    [MaxLength(300)] string? Address,
    [MaxLength(5)] string? BloodGroup);
