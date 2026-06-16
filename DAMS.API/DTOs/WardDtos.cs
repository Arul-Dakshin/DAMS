using System.ComponentModel.DataAnnotations;
using DAMS.Core.Enums;

namespace DAMS.API.DTOs;

public record BedDto(int Id, int WardId, string BedNumber, BedStatus Status);

public record WardDto(
    int Id,
    string Name,
    string? Category,
    int TotalBeds,
    int AvailableBeds,
    List<BedDto> Beds);

public record CreateWardRequest(
    [Required, MaxLength(100)] string Name,
    [MaxLength(50)] string? Category);

public record CreateBedRequest(
    [Required, MaxLength(20)] string BedNumber);

public record UpdateBedStatusRequest([Required] BedStatus Status);
