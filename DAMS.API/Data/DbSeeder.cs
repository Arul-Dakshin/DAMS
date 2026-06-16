using DAMS.Core.Data;
using DAMS.Core.Enums;
using DAMS.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DAMS.API.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(DamsDbContext db)
    {
        // Apply any pending migrations, then seed demo data (each block guarded independently
        // so new seed data lands even on a database that already has earlier seeds).
        await db.Database.MigrateAsync();

        await SeedCoreAsync(db);
        await SeedWardsAsync(db);
    }

    private static async Task SeedWardsAsync(DamsDbContext db)
    {
        if (await db.Wards.AnyAsync())
            return;

        var general = new Ward { Name = "General Ward A", Category = "General" };
        var icu = new Ward { Name = "ICU", Category = "ICU" };
        db.Wards.AddRange(general, icu);
        await db.SaveChangesAsync();

        foreach (var n in new[] { "G-101", "G-102", "G-103", "G-104", "G-105" })
            db.Beds.Add(new Bed { WardId = general.Id, BedNumber = n, Status = BedStatus.Available });
        foreach (var n in new[] { "ICU-1", "ICU-2", "ICU-3" })
            db.Beds.Add(new Bed { WardId = icu.Id, BedNumber = n, Status = BedStatus.Available });
        await db.SaveChangesAsync();
    }

    private static async Task SeedCoreAsync(DamsDbContext db)
    {
        if (await db.Users.AnyAsync())
            return;

        static string Hash(string p) => BCrypt.Net.BCrypt.HashPassword(p);

        var admin = new User { FullName = "System Admin", Email = "admin@dams.com", PasswordHash = Hash("Admin@123"), Role = UserRole.Admin };
        var doctorUser = new User { FullName = "Dr. Alice Carter", Email = "doctor@dams.com", PasswordHash = Hash("Doctor@123"), Role = UserRole.Doctor };
        var reception = new User { FullName = "Rita Reyes", Email = "reception@dams.com", PasswordHash = Hash("Reception@123"), Role = UserRole.Receptionist };
        var patientUser = new User { FullName = "John Miller", Email = "patient@dams.com", PasswordHash = Hash("Patient@123"), Role = UserRole.Patient };

        db.Users.AddRange(admin, doctorUser, reception, patientUser);
        await db.SaveChangesAsync();

        var doctor = new Doctor
        {
            UserId = doctorUser.Id,
            FullName = doctorUser.FullName,
            Specialization = "General Medicine",
            Phone = "555-0100",
            ConsultationFee = 500m
        };
        var patient = new Patient
        {
            UserId = patientUser.Id,
            FullName = patientUser.FullName,
            Gender = Gender.Male,
            DateOfBirth = new DateOnly(1990, 5, 20),
            Phone = "555-0200",
            Address = "12 Main Street",
            BloodGroup = "O+"
        };
        db.Doctors.Add(doctor);
        db.Patients.Add(patient);
        await db.SaveChangesAsync();

        // A Mon–Fri 09:00–13:00 schedule (30-min slots) for the seeded doctor.
        foreach (var day in new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday })
        {
            db.DoctorSchedules.Add(new DoctorSchedule
            {
                DoctorId = doctor.Id,
                DayOfWeek = day,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(13, 0),
                SlotMinutes = 30
            });
        }
        await db.SaveChangesAsync();
    }
}
