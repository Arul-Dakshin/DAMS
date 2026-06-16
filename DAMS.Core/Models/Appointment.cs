using DAMS.Core.Enums;

namespace DAMS.Core.Models;

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime ScheduledStart { get; set; }
    public DateTime ScheduledEnd { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Booked;
    public AppointmentType Type { get; set; } = AppointmentType.OPD;
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Patient Patient { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
}
