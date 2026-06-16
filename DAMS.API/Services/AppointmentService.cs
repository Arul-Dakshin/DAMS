using DAMS.API.DTOs;
using DAMS.Core.Data;
using DAMS.Core.Enums;
using DAMS.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DAMS.API.Services;

public class AppointmentService : IAppointmentService
{
    private readonly DamsDbContext _db;

    public AppointmentService(DamsDbContext db) => _db = db;

    public async Task<List<AppointmentDto>> GetAllAsync(AppointmentStatus? status)
    {
        var query = _db.Appointments.AsNoTracking().AsQueryable();
        if (status is not null)
            query = query.Where(a => a.Status == status);
        return await Project(query.OrderByDescending(a => a.ScheduledStart)).ToListAsync();
    }

    public async Task<List<AppointmentDto>> GetForDoctorUserAsync(int userId)
    {
        var doctorId = await _db.Doctors.Where(d => d.UserId == userId).Select(d => (int?)d.Id).FirstOrDefaultAsync();
        if (doctorId is null) return [];
        return await Project(_db.Appointments.AsNoTracking()
            .Where(a => a.DoctorId == doctorId)
            .OrderByDescending(a => a.ScheduledStart)).ToListAsync();
    }

    public async Task<List<AppointmentDto>> GetForPatientUserAsync(int userId)
    {
        var patientId = await ResolvePatientIdForUserAsync(userId);
        if (patientId is null) return [];
        return await Project(_db.Appointments.AsNoTracking()
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.ScheduledStart)).ToListAsync();
    }

    public async Task<int?> ResolvePatientIdForUserAsync(int userId) =>
        await _db.Patients.Where(p => p.UserId == userId).Select(p => (int?)p.Id).FirstOrDefaultAsync();

    public async Task<BookingResult> CreateAsync(int patientId, CreateAppointmentRequest request)
    {
        if (!await _db.Patients.AnyAsync(p => p.Id == patientId))
            return new BookingResult(null, "Patient not found.");

        if (!await _db.Doctors.AnyAsync(d => d.Id == request.DoctorId))
            return new BookingResult(null, "Doctor not found.");

        if (request.ScheduledStart <= DateTime.Now)
            return new BookingResult(null, "Cannot book a slot in the past.");

        var schedule = await _db.DoctorSchedules.AsNoTracking()
            .FirstOrDefaultAsync(s => s.DoctorId == request.DoctorId && s.DayOfWeek == request.ScheduledStart.DayOfWeek);
        if (schedule is null)
            return new BookingResult(null, "The doctor is not available on that day.");

        var startTime = TimeOnly.FromDateTime(request.ScheduledStart);
        if (startTime < schedule.StartTime || startTime.AddMinutes(schedule.SlotMinutes) > schedule.EndTime)
            return new BookingResult(null, "Selected time is outside the doctor's working hours.");

        var alreadyBooked = await _db.Appointments.AnyAsync(a =>
            a.DoctorId == request.DoctorId &&
            a.Status == AppointmentStatus.Booked &&
            a.ScheduledStart == request.ScheduledStart);
        if (alreadyBooked)
            return new BookingResult(null, "That slot is already booked. Please choose another.", true);

        var appointment = new Appointment
        {
            PatientId = patientId,
            DoctorId = request.DoctorId,
            ScheduledStart = request.ScheduledStart,
            ScheduledEnd = request.ScheduledStart.AddMinutes(schedule.SlotMinutes),
            Status = AppointmentStatus.Booked,
            Type = request.Type,
            Reason = request.Reason?.Trim()
        };
        _db.Appointments.Add(appointment);
        await _db.SaveChangesAsync();

        var dto = await Project(_db.Appointments.AsNoTracking().Where(a => a.Id == appointment.Id)).FirstAsync();
        return new BookingResult(dto);
    }

    public async Task<AppointmentDto?> UpdateStatusAsync(int id, AppointmentStatus status)
    {
        var appointment = await _db.Appointments.FirstOrDefaultAsync(a => a.Id == id);
        if (appointment is null) return null;
        appointment.Status = status;
        await _db.SaveChangesAsync();
        return await Project(_db.Appointments.AsNoTracking().Where(a => a.Id == id)).FirstAsync();
    }

    private static IQueryable<AppointmentDto> Project(IQueryable<Appointment> query) =>
        query.Select(a => new AppointmentDto(
            a.Id,
            a.PatientId,
            a.Patient.FullName,
            a.DoctorId,
            a.Doctor.FullName,
            a.Doctor.Specialization,
            a.ScheduledStart,
            a.ScheduledEnd,
            a.Status,
            a.Type,
            a.Reason));
}
