using SkillHub.Application.DTOs.Authentication;

namespace SkillHub.Application.Interfaces.Authentication;

public interface IAuthenticationService
{
    Task<AuthenticationResponse> RegisterParticipantAsync(
        RegisterRequest request);

    Task<AuthenticationResponse> LoginAsync(
        LoginRequest request);
}