namespace SkillHub.Application.DTOs.Authentication;

/// <summary>
/// Response payload containing authentication JWT token and user identity claims.
/// </summary>
public class AuthenticationResponse
{
    /// <summary>The signed JWT Bearer access token string.</summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>The UTC expiration timestamp of the token.</summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>The authenticated user's email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>The security role assigned to the user (Participant or Employee).</summary>
    public string Role { get; set; } = string.Empty;
}
