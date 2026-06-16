using DAMS.API.DTOs;
using DAMS.Core.Data;
using DAMS.Core.Enums;
using DAMS.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DAMS.API.Services;

public class DoctorService : IDoctorService
{
    private readonly DamsDbContext _db;

    public DoctorService(DamsDbContext db) => _db = db;

    public async Task<List<DoctorDto>> GetAllAsync(string? search)
    {
        var query = _db.Doctors.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(d => d.FullName.Contains(term) || d.Specialization.Contains(term));
        }
        return await query.OrderBy(d => d.FullName).Select(d => Map(d)).ToListAsync();
    }

    public async Task<DoctorDto?> GetByIdAsync(int id)
    {
        var doctor = await _db.Doctors.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
        return doctor is null ? null : Map(doctor);
    }

    public async Task<DoctorDto> CreateAsync(CreateDoctorRequest request)
    {
        var doctor = new Doctor
        {
            FullName = request.FullName.Trim(),
            Specialization = request.Specialization.Trim(),
            Phone = request.Phone?.Trim(),
            ConsultationFee = request.ConsultationFee
        };
        _db.Doctors.Add(doctor);
        await _db.SaveChangesAsync();
        return Map(doctor);
    }

    public async Task<DoctorDto?> UpdateAsync(int id, UpdateDoctorRequest request)
    {
        var doctor = await _db.Doctors.FirstOrDefaultAsync(d => d.Id == id);
        if (doctor is null) return null;

        doctor.FullName = request.FullName.Trim();
        doctor.Specialization = request.Specialization.Trim();
        doctor.Phone = request.Phone?.Trim();
        doctor.ConsultationFee = request.ConsultationFee;
        await _db.SaveChangesAsync();
        return Map(doctor);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var doctor = await _db.Doctors.FirstOrDefaultAsync(d => d.Id == id);
        if (doctor is null) return false;
        _db.Doctors.Remove(doctor);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<DoctorScheduleDto>> GetSchedulesAsync(int doctorId)
    {
        return await _db.DoctorSchedules.AsNoTracking()
            .Where(s => s.DoctorId == doctorId)
            .OrderBy(s => s.DayOfWeek).ThenBy(s => s.StartTime)
            .Select(s => new DoctorScheduleDto(s.Id, s.DoctorId, s.DayOfWeek, s.StartTime, s.EndTime, s.SlotMinutes))
            .ToListAsync();
    }

    public async Task<DoctorScheduleDto?> AddScheduleAsync(int doctorId, CreateScheduleRequest request)
    {
        if (!await _db.Doctors.AnyAsync(d => d.Id == doctorId)) return null;
        if (request.EndTime <= request.StartTime) return null;

        var schedule = new DoctorSchedule
        {
            DoctorId = doctorId,
            DayOfWeek = request.DayOfWeek,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            SlotMinutes = request.SlotMinutes
        };
        _db.DoctorSchedules.Add(schedule);
        await _db.SaveChangesAsync();
        return new DoctorScheduleDto(schedule.Id, schedule.DoctorId, schedule.DayOfWeek, schedule.StartTime, schedule.EndTime, schedule.SlotMinutes);
    }

    public async Task<bool> DeleteScheduleAsync(int doctorId, int scheduleId)
    {
        var schedule = await _db.DoctorSchedules.FirstOrDefaultAsync(s => s.Id == scheduleId && s.DoctorId == doctorId);
        if (schedule is null) return false;
        _db.DoctorSchedules.Remove(schedule);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<SlotDto>> GetAvailableSlotsAsync(int doctorId, DateOnly date)
    {
        var schedules = await _db.DoctorSchedules.AsNoTracking()
            .Where(s => s.DoctorId == doctorId && s.DayOfWeek == date.DayOfWeek)
            .ToListAsync();
        if (schedules.Count == 0) return [];

        var dayStart = date.ToDateTime(TimeOnly.MinValue);
        var dayEnd = dayStart.AddDays(1);
        var booked = await _db.Appointments.AsNoTracking()
            .Where(a => a.DoctorId == doctorId
                        && a.Status == AppointmentStatus.Booked
                        && a.ScheduledStart >= dayStart && a.ScheduledStart < dayEnd)
            .Select(a => a.ScheduledStart)
            .ToListAsync();

        var now = DateTime.Now;
        var slots = new List<SlotDto>();
        foreach (var sch in schedules)
        {
            var cursor = date.ToDateTime(sch.StartTime);
            var windowEnd = date.ToDateTime(sch.EndTime);
            while (cursor.AddMinutes(sch.SlotMinutes) <= windowEnd)
            {
                var slotEnd = cursor.AddMinutes(sch.SlotMinutes);
                if (cursor > now && !booked.Contains(cursor))
                    slots.Add(new SlotDto(cursor, slotEnd));
                cursor = slotEnd;
            }
        }
        return slots.OrderBy(s => s.Start).ToList();
    }

    private static DoctorDto Map(Doctor d) =>
        new(d.Id, d.FullName, d.Specialization, d.Phone, d.ConsultationFee, d.UserId);
}
