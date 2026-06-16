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
    public DbSet<Ward> Wards => Set<Ward>();
    public DbSet<Bed> Beds => Set<Bed>();
    public DbSet<Admission> Admissions => Set<Admission>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();

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

        modelBuilder.Entity<Ward>(e =>
        {
            e.Property(w => w.Name).HasMaxLength(100).IsRequired();
            e.Property(w => w.Category).HasMaxLength(50);
        });

        modelBuilder.Entity<Bed>(e =>
        {
            e.Property(b => b.BedNumber).HasMaxLength(20).IsRequired();
            e.Property(b => b.Status).HasConversion<string>().HasMaxLength(15);
            e.HasIndex(b => new { b.WardId, b.BedNumber }).IsUnique();

            e.HasOne(b => b.Ward)
                .WithMany(w => w.Beds)
                .HasForeignKey(b => b.WardId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Admission>(e =>
        {
            e.Property(a => a.Status).HasConversion<string>().HasMaxLength(15);
            e.Property(a => a.Reason).HasMaxLength(500);
            e.HasIndex(a => new { a.PatientId, a.Status });

            e.HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(a => a.Bed)
                .WithMany(b => b.Admissions)
                .HasForeignKey(a => a.BedId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Prescription>(e =>
        {
            e.Property(p => p.Diagnosis).HasMaxLength(1000).IsRequired();
            e.Property(p => p.Notes).HasMaxLength(1000);

            // One prescription per appointment.
            e.HasOne(p => p.Appointment)
                .WithOne()
                .HasForeignKey<Prescription>(p => p.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(p => p.Doctor)
                .WithMany()
                .HasForeignKey(p => p.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(p => p.Patient)
                .WithMany()
                .HasForeignKey(p => p.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PrescriptionItem>(e =>
        {
            e.Property(i => i.MedicineName).HasMaxLength(150).IsRequired();
            e.Property(i => i.Dosage).HasMaxLength(50).IsRequired();
            e.Property(i => i.Frequency).HasMaxLength(50).IsRequired();
            e.Property(i => i.Instructions).HasMaxLength(200);

            e.HasOne(i => i.Prescription)
                .WithMany(p => p.Items)
                .HasForeignKey(i => i.PrescriptionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Invoice>(e =>
        {
            e.Property(i => i.Status).HasConversion<string>().HasMaxLength(15);
            e.Property(i => i.Notes).HasMaxLength(500);

            e.HasOne(i => i.Patient)
                .WithMany()
                .HasForeignKey(i => i.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Optional link to the appointment that was billed.
            e.HasOne<Appointment>()
                .WithMany()
                .HasForeignKey(i => i.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<InvoiceItem>(e =>
        {
            e.Property(i => i.Description).HasMaxLength(200).IsRequired();
            e.Property(i => i.UnitPrice).HasPrecision(18, 2);

            e.HasOne(i => i.Invoice)
                .WithMany(inv => inv.Items)
                .HasForeignKey(i => i.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
