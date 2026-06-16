using DAMS.API.DTOs;
using DAMS.Core.Data;
using DAMS.Core.Enums;
using DAMS.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DAMS.API.Services;

public class AdmissionService : IAdmissionService
{
    private readonly DamsDbContext _db;

    public AdmissionService(DamsDbContext db) => _db = db;

    public async Task<List<AdmissionDto>> GetAllAsync(AdmissionStatus? status)
    {
        var query = _db.Admissions.AsNoTracking().AsQueryable();
        if (status is not null)
            query = query.Where(a => a.Status == status);
        return await Project(query.OrderByDescending(a => a.AdmitDate)).ToListAsync();
    }

    public async Task<AdmissionResult> AdmitAsync(CreateAdmissionRequest request)
    {
        if (!await _db.Patients.AnyAsync(p => p.Id == request.PatientId))
            return new AdmissionResult(null, "Patient not found.");

        var bed = await _db.Beds.FirstOrDefaultAsync(b => b.Id == request.BedId);
        if (bed is null)
            return new AdmissionResult(null, "Bed not found.");
        if (bed.Status != BedStatus.Available)
            return new AdmissionResult(null, "That bed is not available.", true);

        var alreadyAdmitted = await _db.Admissions.AnyAsync(a =>
            a.PatientId == request.PatientId && a.Status == AdmissionStatus.Admitted);
        if (alreadyAdmitted)
            return new AdmissionResult(null, "Patient is already admitted.", true);

        var admission = new Admission
        {
            PatientId = request.PatientId,
            BedId = request.BedId,
            AdmitDate = DateTime.UtcNow,
            Status = AdmissionStatus.Admitted,
            Reason = request.Reason?.Trim()
        };
        bed.Status = BedStatus.Occupied;
        _db.Admissions.Add(admission);
        await _db.SaveChangesAsync();

        var dto = await Project(_db.Admissions.AsNoTracking().Where(a => a.Id == admission.Id)).FirstAsync();
        return new AdmissionResult(dto);
    }

    public async Task<AdmissionDto?> DischargeAsync(int admissionId)
    {
        var admission = await _db.Admissions
            .Include(a => a.Bed)
            .FirstOrDefaultAsync(a => a.Id == admissionId);
        if (admission is null || admission.Status == AdmissionStatus.Discharged)
            return null;

        admission.Status = AdmissionStatus.Discharged;
        admission.DischargeDate = DateTime.UtcNow;
        admission.Bed.Status = BedStatus.Available;
        await _db.SaveChangesAsync();

        return await Project(_db.Admissions.AsNoTracking().Where(a => a.Id == admissionId)).FirstAsync();
    }

    private static IQueryable<AdmissionDto> Project(IQueryable<Admission> query) =>
        query.Select(a => new AdmissionDto(
            a.Id,
            a.PatientId,
            a.Patient.FullName,
            a.BedId,
            a.Bed.BedNumber,
            a.Bed.Ward.Name,
            a.AdmitDate,
            a.DischargeDate,
            a.Status,
            a.Reason));
}
