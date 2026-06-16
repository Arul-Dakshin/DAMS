using DAMS.Core.Enums;

namespace DAMS.Core.Models;

public class Bed
{
    public int Id { get; set; }
    public int WardId { get; set; }
    public string BedNumber { get; set; } = string.Empty;
    public BedStatus Status { get; set; } = BedStatus.Available;

    public Ward Ward { get; set; } = null!;
    public ICollection<Admission> Admissions { get; set; } = new List<Admission>();
}
