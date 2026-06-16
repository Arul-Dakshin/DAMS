using DAMS.API.DTOs;
using DAMS.Core.Data;
using DAMS.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DAMS.API.Services;

public class PatientService : IPatientService
{
    private readonly DamsDbContext _db;

    public PatientService(DamsDbContext db) => _db = db;

    public async Task<List<PatientDto>> GetAllAsync(string? search)
    {
        var query = _db.Patients.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p => p.FullName.Contains(term) || p.Phone.Contains(term));
        }

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => Map(p))
            .ToListAsync();
    }

    public async Task<PatientDto?> GetByIdAsync(int id)
    {
        var patient = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        return patient is null ? null : Map(patient);
    }

    public async Task<PatientDto?> GetByUserIdAsync(int userId)
    {
        var patient = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.UserId == userId);
        return patient is null ? null : Map(patient);
    }

    public async Task<PatientDto> CreateAsync(CreatePatientRequest request)
    {
        var patient = new Patient
        {
            FullName = request.FullName.Trim(),
            Gender = request.Gender,
            DateOfBirth = request.DateOfBirth,
            Phone = request.Phone.Trim(),
            Address = request.Address?.Trim(),
            BloodGroup = request.BloodGroup?.Trim()
        };
        _db.Patients.Add(patient);
        await _db.SaveChangesAsync();
        return Map(patient);
    }

    public async Task<PatientDto?> UpdateAsync(int id, UpdatePatientRequest request)
    {
        var patient = await _db.Patients.FirstOrDefaultAsync(p => p.Id == id);
        if (patient is null) return null;

        patient.FullName = request.FullName.Trim();
        patient.Gender = request.Gender;
        patient.DateOfBirth = request.DateOfBirth;
        patient.Phone = request.Phone.Trim();
        patient.Address = request.Address?.Trim();
        patient.BloodGroup = request.BloodGroup?.Trim();

        await _db.SaveChangesAsync();
        return Map(patient);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var patient = await _db.Patients.FirstOrDefaultAsync(p => p.Id == id);
        if (patient is null) return false;
        _db.Patients.Remove(patient);
        await _db.SaveChangesAsync();
        return true;
    }

    private static PatientDto Map(Patient p) =>
        new(p.Id, p.FullName, p.Gender, p.DateOfBirth, p.Phone, p.Address, p.BloodGroup, p.UserId, p.CreatedAt);
}
