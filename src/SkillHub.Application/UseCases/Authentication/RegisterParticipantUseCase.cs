using SkillHub.Application.DTOs.Authentication;
using SkillHub.Application.Interfaces.Authentication;

namespace SkillHub.Application.UseCases.Authentication.RegisterParticipant;

/// <summary>
/// Use case orchestrating participant registration, field validation, and account generation.
/// </summary>
public class RegisterParticipantUseCase
{
    private readonly IAuthenticationService _authenticationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterParticipantUseCase"/> class.
    /// </summary>
    public RegisterParticipantUseCase(
        IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    /// <summary>
    /// Validates registration details and invokes the authentication service to register the participant.
    /// </summary>
    /// <param name="request">The registration request payload.</param>
    /// <returns>The authentication response with issued JWT token.</returns>
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
