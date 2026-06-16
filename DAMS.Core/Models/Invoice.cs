using DAMS.Core.Enums;

namespace DAMS.Core.Models;

public class Invoice
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int? AppointmentId { get; set; }
    public DateTime IssuedDate { get; set; } = DateTime.UtcNow;
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;
    public DateTime? PaidDate { get; set; }
    public string? Notes { get; set; }

    public Patient Patient { get; set; } = null!;
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
}
