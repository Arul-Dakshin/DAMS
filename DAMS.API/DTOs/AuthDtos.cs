using System.ComponentModel.DataAnnotations;

namespace DAMS.API.DTOs;

public record RegisterRequest(
    [Required, MaxLength(150)] string FullName,
    [Required, EmailAddress, MaxLength(150)] string Email,
    [Required, MinLength(6)] string Password);

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);

public record UserInfo(int Id, string FullName, string Email, string Role);

public record AuthResponse(string Token, DateTime ExpiresAtUtc, UserInfo User);
