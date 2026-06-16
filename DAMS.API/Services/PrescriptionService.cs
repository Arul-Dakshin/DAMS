using DAMS.API.Authorization;
using DAMS.API.DTOs;
using DAMS.Core.Data;
using DAMS.Core.Enums;
using DAMS.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DAMS.API.Services;

public class PrescriptionService : IPrescriptionService
{
    private readonly DamsDbContext _db;

    public PrescriptionService(DamsDbContext db) => _db = db;

    public async Task<PrescriptionResult> CreateAsync(int userId, CreatePrescriptionRequest request)
    {
        var doctorId = await _db.Doctors.Where(d => d.UserId == userId).Select(d => (int?)d.Id).FirstOrDefaultAsync();
        if (doctorId is null)
            return new PrescriptionResult(null, "Your account is not linked to a doctor profile.");

        var appointment = await _db.Appointments.FirstOrDefaultAsync(a => a.Id == request.AppointmentId);
        if (appointment is null)
            return new PrescriptionResult(null, "Appointment not found.");
        if (appointment.DoctorId != doctorId)
            return new PrescriptionResult(null, "You can only prescribe for your own appointments.");

        if (await _db.Prescriptions.AnyAsync(p => p.AppointmentId == request.AppointmentId))
            return new PrescriptionResult(null, "A prescription already exists for this appointment.", true);

        var prescription = new Prescription
        {
            AppointmentId = appointment.Id,
            DoctorId = doctorId.Value,
            PatientId = appointment.PatientId,
            Diagnosis = request.Diagnosis.Trim(),
            Notes = request.Notes?.Trim(),
            Items = request.Items.Select(i => new PrescriptionItem
            {
                MedicineName = i.MedicineName.Trim(),
                Dosage = i.Dosage.Trim(),
                Frequency = i.Frequency.Trim(),
                DurationDays = i.DurationDays,
                Instructions = i.Instructions?.Trim()
            }).ToList()
        };

        // Recording a prescription closes out the visit.
        appointment.Status = AppointmentStatus.Completed;

        _db.Prescriptions.Add(prescription);
        await _db.SaveChangesAsync();

        var dto = await Project(_db.Prescriptions.AsNoTracking().Where(p => p.Id == prescription.Id)).FirstAsync();
        return new PrescriptionResult(dto);
    }

    public async Task<List<PrescriptionDto>> GetForUserAsync(string role, int userId)
    {
        var query = _db.Prescriptions.AsNoTracking().AsQueryable();

        if (role == Roles.Doctor)
        {
            var doctorId = await _db.Doctors.Where(d => d.UserId == userId).Select(d => (int?)d.Id).FirstOrDefaultAsync();
            query = query.Where(p => p.DoctorId == doctorId);
        }
        else if (role == Roles.Patient)
        {
            var patientId = await _db.Patients.Where(p => p.UserId == userId).Select(p => (int?)p.Id).FirstOrDefaultAsync();
            query = query.Where(p => p.PatientId == patientId);
        }
        // Admin (and any other staff) see all.

        return await Project(query.OrderByDescending(p => p.CreatedAt)).ToListAsync();
    }

    public async Task<PrescriptionDto?> GetByIdAsync(int id, string role, int userId)
    {
        var dto = await Project(_db.Prescriptions.AsNoTracking().Where(p => p.Id == id)).FirstOrDefaultAsync();
        if (dto is null) return null;

        if (role == Roles.Doctor)
        {
            var doctorId = await _db.Doctors.Where(d => d.UserId == userId).Select(d => (int?)d.Id).FirstOrDefaultAsync();
            if (dto.DoctorId != doctorId) return null;
        }
        else if (role == Roles.Patient)
        {
            var patientId = await _db.Patients.Where(p => p.UserId == userId).Select(p => (int?)p.Id).FirstOrDefaultAsync();
            if (dto.PatientId != patientId) return null;
        }

        return dto;
    }

    public async Task<PrescriptionDto?> GetByAppointmentAsync(int appointmentId) =>
        await Project(_db.Prescriptions.AsNoTracking().Where(p => p.AppointmentId == appointmentId)).FirstOrDefaultAsync();

    private static IQueryable<PrescriptionDto> Project(IQueryable<Prescription> query) =>
        query.Select(p => new PrescriptionDto(
            p.Id,
            p.AppointmentId,
            p.DoctorId,
            p.Doctor.FullName,
            p.PatientId,
            p.Patient.FullName,
            p.Diagnosis,
            p.Notes,
            p.CreatedAt,
            p.Appointment.ScheduledStart,
            p.Items.Select(i => new PrescriptionItemDto(i.Id, i.MedicineName, i.Dosage, i.Frequency, i.DurationDays, i.Instructions)).ToList()));
}
