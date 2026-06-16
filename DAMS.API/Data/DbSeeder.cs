using DAMS.Core.Data;
using DAMS.Core.Enums;
using DAMS.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DAMS.API.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(DamsDbContext db)
    {
        // Apply any pending migrations, then seed demo data once.
        await db.Database.MigrateAsync();

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
