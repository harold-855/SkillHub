using Microsoft.AspNetCore.Identity;

namespace SkillHub.Infrastructure.Identity;

/// <summary>
/// Custom ASP.NET Core Identity user entity using Guid as primary key.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>The legal full name of the user.</summary>
    public string FullName { get; set; } = string.Empty;
}
