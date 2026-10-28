using Microsoft.AspNetCore.Mvc;
using SkillHub.Application.DTOs.Authentication;
using SkillHub.Application.UseCases.Authentication.Login;
using SkillHub.Application.UseCases.Authentication.RegisterParticipant;

namespace SkillHub.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterParticipantUseCase _registerParticipantUseCase;
    private readonly LoginUseCase _loginUseCase;

    public AuthController(
        RegisterParticipantUseCase registerParticipantUseCase,
        LoginUseCase loginUseCase)
    {
        _registerParticipantUseCase = registerParticipantUseCase;
        _loginUseCase = loginUseCase;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request)
    {
        var response =
            await _registerParticipantUseCase.ExecuteAsync(request);

        return Created("api/v1/auth/register", response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        var response =
            await _loginUseCase.ExecuteAsync(request);

        return Ok(response);
    }
}