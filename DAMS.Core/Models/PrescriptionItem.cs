namespace DAMS.Core.Models;

public class PrescriptionItem
{
    public int Id { get; set; }
    public int PrescriptionId { get; set; }
    public string MedicineName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;      // e.g. "500 mg"
    public string Frequency { get; set; } = string.Empty;   // e.g. "Twice daily"
    public int DurationDays { get; set; }
    public string? Instructions { get; set; }               // e.g. "After meals"

    public Prescription Prescription { get; set; } = null!;
}
