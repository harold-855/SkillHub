namespace SkillHub.Application.DTOs.Authentication;

/// <summary>
/// Payload submitted to authenticate an existing user.
/// </summary>
public class LoginRequest
{
    /// <summary>The registered user's email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>The user's secret password.</summary>
    public string Password { get; set; } = string.Empty;
}
