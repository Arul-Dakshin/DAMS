using DAMS.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DAMS.Core.Data;

public class DamsDbContext : DbContext
{
    public DamsDbContext(DbContextOptions<DamsDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<DoctorSchedule> DoctorSchedules => Set<DoctorSchedule>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.FullName).HasMaxLength(150).IsRequired();
            e.Property(u => u.Email).HasMaxLength(150).IsRequired();
            e.Property(u => u.PasswordHash).IsRequired();
            e.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);
        });

        modelBuilder.Entity<Patient>(e =>
        {
            e.Property(p => p.FullName).HasMaxLength(150).IsRequired();
            e.Property(p => p.Phone).HasMaxLength(20).IsRequired();
            e.Property(p => p.Address).HasMaxLength(300);
            e.Property(p => p.BloodGroup).HasMaxLength(5);
            e.Property(p => p.Gender).HasConversion<string>().HasMaxLength(10);

            e.HasOne(p => p.User)
                .WithOne(u => u.Patient)
                .HasForeignKey<Patient>(p => p.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Doctor>(e =>
        {
            e.Property(d => d.FullName).HasMaxLength(150).IsRequired();
            e.Property(d => d.Specialization).HasMaxLength(100).IsRequired();
            e.Property(d => d.Phone).HasMaxLength(20);
            e.Property(d => d.ConsultationFee).HasPrecision(18, 2);

            e.HasOne(d => d.User)
                .WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<DoctorSchedule>(e =>
        {
            e.Property(s => s.DayOfWeek).HasConversion<string>().HasMaxLength(10);

            e.HasOne(s => s.Doctor)
                .WithMany(d => d.Schedules)
                .HasForeignKey(s => s.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Appointment>(e =>
        {
            e.Property(a => a.Status).HasConversion<string>().HasMaxLength(15);
            e.Property(a => a.Type).HasConversion<string>().HasMaxLength(5);
            e.Property(a => a.Reason).HasMaxLength(500);
            e.HasIndex(a => new { a.DoctorId, a.ScheduledStart });

            e.HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
