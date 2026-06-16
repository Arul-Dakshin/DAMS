using DAMS.API.DTOs;
using DAMS.Core.Data;
using DAMS.Core.Enums;
using DAMS.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DAMS.API.Services;

public class WardService : IWardService
{
    private readonly DamsDbContext _db;

    public WardService(DamsDbContext db) => _db = db;

    public async Task<List<WardDto>> GetWardsAsync()
    {
        var wards = await _db.Wards.AsNoTracking()
            .Include(w => w.Beds)
            .OrderBy(w => w.Name)
            .ToListAsync();
        return wards.Select(MapWard).ToList();
    }

    public async Task<WardDto> CreateWardAsync(CreateWardRequest request)
    {
        var ward = new Ward { Name = request.Name.Trim(), Category = request.Category?.Trim() };
        _db.Wards.Add(ward);
        await _db.SaveChangesAsync();
        return MapWard(ward);
    }

    public async Task<bool> DeleteWardAsync(int id)
    {
        var ward = await _db.Wards.FirstOrDefaultAsync(w => w.Id == id);
        if (ward is null) return false;
        _db.Wards.Remove(ward); // beds cascade-delete
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<BedDto?> AddBedAsync(int wardId, CreateBedRequest request)
    {
        if (!await _db.Wards.AnyAsync(w => w.Id == wardId)) return null;

        var number = request.BedNumber.Trim();
        if (await _db.Beds.AnyAsync(b => b.WardId == wardId && b.BedNumber == number)) return null;

        var bed = new Bed { WardId = wardId, BedNumber = number, Status = BedStatus.Available };
        _db.Beds.Add(bed);
        await _db.SaveChangesAsync();
        return new BedDto(bed.Id, bed.WardId, bed.BedNumber, bed.Status);
    }

    public async Task<bool> DeleteBedAsync(int wardId, int bedId)
    {
        var bed = await _db.Beds.FirstOrDefaultAsync(b => b.Id == bedId && b.WardId == wardId);
        if (bed is null || bed.Status == BedStatus.Occupied) return false;
        _db.Beds.Remove(bed);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<BedDto?> UpdateBedStatusAsync(int bedId, UpdateBedStatusRequest request)
    {
        var bed = await _db.Beds.FirstOrDefaultAsync(b => b.Id == bedId);
        if (bed is null) return null;

        // Occupancy is driven by admissions; only allow toggling between Available and Maintenance here.
        if (bed.Status == BedStatus.Occupied || request.Status == BedStatus.Occupied) return null;

        bed.Status = request.Status;
        await _db.SaveChangesAsync();
        return new BedDto(bed.Id, bed.WardId, bed.BedNumber, bed.Status);
    }

    public async Task<List<BedDto>> GetAvailableBedsAsync()
    {
        return await _db.Beds.AsNoTracking()
            .Where(b => b.Status == BedStatus.Available)
            .OrderBy(b => b.WardId).ThenBy(b => b.BedNumber)
            .Select(b => new BedDto(b.Id, b.WardId, b.BedNumber, b.Status))
            .ToListAsync();
    }

    private static WardDto MapWard(Ward w)
    {
        var beds = w.Beds
            .OrderBy(b => b.BedNumber)
            .Select(b => new BedDto(b.Id, b.WardId, b.BedNumber, b.Status))
            .ToList();
        return new WardDto(w.Id, w.Name, w.Category, beds.Count, beds.Count(b => b.Status == BedStatus.Available), beds);
    }
}
