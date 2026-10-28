namespace SkillHub.Application.DTOs.Authentication;

/// <summary>
/// Payload submitted to register a new participant user.
/// </summary>
public class RegisterRequest
{
    /// <summary>The legal full name of the participant.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>The unique email address used for login.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>The chosen password.</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>Password confirmation matching Password.</summary>
    public string ConfirmPassword { get; set; } = string.Empty;
}
