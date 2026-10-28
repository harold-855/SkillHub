namespace SkillHub.Application.Interfaces.Authentication;

/// <summary>
/// Service abstraction for creating cryptographically signed JWT security tokens.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates a signed JWT bearer token containing user claims and assigned roles.
    /// </summary>
    Task<(string AccessToken, DateTime ExpiresAt)> GenerateTokenAsync(
        Guid userId,
        string email,
        IEnumerable<string> roles);
}
