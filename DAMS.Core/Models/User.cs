using DAMS.Core.Enums;

namespace DAMS.Core.Models;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Optional 1:1 links — a login may belong to a patient or a doctor profile.
    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
}
