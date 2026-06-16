using DAMS.API.DTOs;

namespace DAMS.API.Services;

public interface IWardService
{
    Task<List<WardDto>> GetWardsAsync();
    Task<WardDto> CreateWardAsync(CreateWardRequest request);
    Task<bool> DeleteWardAsync(int id);

    Task<BedDto?> AddBedAsync(int wardId, CreateBedRequest request);
    Task<bool> DeleteBedAsync(int wardId, int bedId);
    Task<BedDto?> UpdateBedStatusAsync(int bedId, UpdateBedStatusRequest request);

    Task<List<BedDto>> GetAvailableBedsAsync();
}
