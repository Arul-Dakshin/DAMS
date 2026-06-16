namespace DAMS.Core.Models;

public class Ward
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; } // e.g. General, ICU, Private

    public ICollection<Bed> Beds { get; set; } = new List<Bed>();
}
