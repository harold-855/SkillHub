using SkillHub.Application.DTOs.Authentication;
using SkillHub.Application.Interfaces.Authentication;

namespace SkillHub.Application.UseCases.Authentication.Login;

/// <summary>
/// Use case executing user authentication and credential verification.
/// </summary>
public class LoginUseCase
{
    private readonly IAuthenticationService _authenticationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginUseCase"/> class.
    /// </summary>
    public LoginUseCase(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    /// <summary>
    /// Validates login input and requests a JWT authentication response from the identity provider.
    /// </summary>
    /// <param name="request">The login request payload.</param>
    /// <returns>The issued token and user role payload.</returns>
    public async Task<AuthenticationResponse> ExecuteAsync(
        LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("Password is required.");
        }

        return await _authenticationService.LoginAsync(request);
    }
}
