
namespace SkillHub.Application.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    Task<(string AccessToken, DateTime ExpiresAt)> GenerateTokenAsync(
        Guid userId,
        string email,
        IEnumerable<string> roles);
}