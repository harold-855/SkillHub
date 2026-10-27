using SkillHub.Application.DTOs.Authentication;
using SkillHub.Application.Interfaces.Authentication;

namespace SkillHub.Application.UseCases.Authentication.RegisterParticipant;

public class RegisterParticipantUseCase
{
    private readonly IAuthenticationService _authenticationService;

    public RegisterParticipantUseCase(
        IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<AuthenticationResponse> ExecuteAsync(
        RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            throw new ArgumentException(
                "Full name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException(
                "Email is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException(
                "Password is required.");
        }

        if (request.Password != request.ConfirmPassword)
        {
            throw new ArgumentException(
                "Password and confirmation password do not match.");
        }

        return await _authenticationService
            .RegisterParticipantAsync(request);
    }
}