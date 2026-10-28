using SkillHub.Application.DTOs.Authentication;

namespace SkillHub.Application.Interfaces.Authentication;

/// <summary>
/// Service abstraction for participant registration and user authentication.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Registers a new participant account and generates an initial JWT token response.
    /// </summary>
    Task<AuthenticationResponse> RegisterParticipantAsync(
        RegisterRequest request);

    /// <summary>
    /// Validates user credentials and produces an authenticated JWT token response.
    /// </summary>
    Task<AuthenticationResponse> LoginAsync(
        LoginRequest request);
}
