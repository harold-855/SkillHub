using SkillHub.Application.DTOs.Authentication;
using SkillHub.Application.Interfaces.Authentication;

namespace SkillHub.Application.UseCases.Authentication.Login;

public class LoginUseCase
{
    private readonly IAuthenticationService _authenticationService;

    public LoginUseCase(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

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