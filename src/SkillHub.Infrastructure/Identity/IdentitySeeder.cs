using Microsoft.AspNetCore.Identity;

namespace SkillHub.Infrastructure.Identity;

/// <summary>
/// Seed component initializing essential Identity roles (Participant and Employee) upon application startup.
/// </summary>
public class IdentitySeeder
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentitySeeder"/> class.
    /// </summary>
    public IdentitySeeder(
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _roleManager = roleManager;
    }

    /// <summary>
    /// Verifies and creates the default system roles if they do not exist.
    /// </summary>
    public async Task SeedRolesAsync()
    {
        string[] roles =
        [
            "Participant",
            "Employee"
        ];

        foreach (var role in roles)
        {
            var roleExists = await _roleManager.RoleExistsAsync(role);

            if (!roleExists)
            {
                var result = await _roleManager.CreateAsync(
                    new IdentityRole<Guid>(role));

                if (!result.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        result.Errors.Select(error => error.Description));

                    throw new InvalidOperationException(
                        $"Could not create role '{role}': {errors}");
                }
            }
        }
    }
}
