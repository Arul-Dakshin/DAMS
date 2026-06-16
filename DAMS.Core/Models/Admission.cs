using DAMS.Core.Enums;

namespace DAMS.Core.Models;

public class Admission
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int BedId { get; set; }
    public DateTime AdmitDate { get; set; } = DateTime.UtcNow;
    public DateTime? DischargeDate { get; set; }
    public AdmissionStatus Status { get; set; } = AdmissionStatus.Admitted;
    public string? Reason { get; set; }

    public Patient Patient { get; set; } = null!;
    public Bed Bed { get; set; } = null!;
}
