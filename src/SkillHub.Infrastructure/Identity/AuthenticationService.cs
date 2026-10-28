using Microsoft.AspNetCore.Identity;
using SkillHub.Application.DTOs.Authentication;
using SkillHub.Application.Interfaces.Authentication;
using SkillHub.Application.Interfaces.Repositories;
using SkillHub.Domain.Entities;

namespace SkillHub.Infrastructure.Identity;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IParticipantRepository _participantRepository;
    private readonly IInstructorRepository _instructorRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthenticationService(
        UserManager<ApplicationUser> userManager,
        IParticipantRepository participantRepository,
        IInstructorRepository instructorRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _participantRepository = participantRepository;
        _instructorRepository = instructorRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthenticationResponse> RegisterParticipantAsync(
        RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            throw new ArgumentException("Full name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("Password is required.");
        }

        if (request.Password != request.ConfirmPassword)
        {
            throw new ArgumentException("Passwords do not match.");
        }

        var email = request.Email.Trim().ToLowerInvariant();

        if (await _userManager.FindByEmailAsync(email) is not null)
        {
            throw new InvalidOperationException(
                "An account with this email already exists.");
        }

        if (await _participantRepository.ExistsByEmailAsync(email))
        {
            throw new InvalidOperationException(
                "A participant with this email already exists.");
        }

        if (await _instructorRepository.ExistsByEmailAsync(email))
        {
            throw new InvalidOperationException(
                "This email belongs to an existing instructor.");
        }

        var participant = new Participant(
            request.FullName,
            email);

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = request.FullName.Trim()
        };

        var createUserResult = await _userManager.CreateAsync(
            user,
            request.Password);

        if (!createUserResult.Succeeded)
        {
            var errors = string.Join(
                "; ",
                createUserResult.Errors.Select(error => error.Description));

            throw new InvalidOperationException(
                $"Could not create the account: {errors}");
        }

        var roleResult = await _userManager.AddToRoleAsync(
            user,
            "Participant");

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            var errors = string.Join(
                "; ",
                roleResult.Errors.Select(error => error.Description));

            throw new InvalidOperationException(
                $"Could not assign the Participant role: {errors}");
        }

        try
        {
            await _participantRepository.AddAsync(participant);
        }
        catch
        {
            await _userManager.DeleteAsync(user);
            throw;
        }

        var roles = await _userManager.GetRolesAsync(user);

        var (accessToken, expiresAt) =
            await _jwtTokenGenerator.GenerateTokenAsync(
                participant.Id,
                email,
                roles);

        return new AuthenticationResponse
        {
            AccessToken = accessToken,
            ExpiresAt = expiresAt,
            Email = email,
            Role = "Participant"
        };
    }

    public async Task<AuthenticationResponse> LoginAsync(
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

        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null ||
            !await _userManager.CheckPasswordAsync(
                user,
                request.Password))
        {
            throw new UnauthorizedAccessException(
                "The email or password is incorrect.");
        }

        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Count == 0)
        {
            throw new InvalidOperationException(
                "The account does not have an assigned role.");
        }

        Guid domainUserId;

        if (roles.Contains("Participant"))
        {
            var participant =
                await _participantRepository.GetByEmailAsync(email);

            if (participant is null)
            {
                throw new InvalidOperationException(
                    "The participant profile was not found.");
            }

            domainUserId = participant.Id;
        }
        else if (roles.Contains("Employee"))
        {
            var instructor =
                await _instructorRepository.GetByEmailAsync(email);

            if (instructor is null)
            {
                throw new InvalidOperationException(
                    "The instructor profile was not found.");
            }

            domainUserId = instructor.Id;
        }
        else
        {
            throw new UnauthorizedAccessException(
                "The account does not have a supported role.");
        }

        var (accessToken, expiresAt) =
            await _jwtTokenGenerator.GenerateTokenAsync(
                domainUserId,
                email,
                roles);

        return new AuthenticationResponse
        {
            AccessToken = accessToken,
            ExpiresAt = expiresAt,
            Email = email,
            Role = roles[0]
        };
    }
}
