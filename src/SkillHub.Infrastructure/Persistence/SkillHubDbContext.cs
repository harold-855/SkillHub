using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkillHub.Domain.Entities;
using SkillHub.Infrastructure.Identity;

namespace SkillHub.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core database context extending IdentityDbContext to manage both
/// application domain entities and ASP.NET Core Identity tables within PostgreSQL.
/// </summary>
public class SkillHubDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SkillHubDbContext"/> class.
    /// </summary>
    public SkillHubDbContext(
        DbContextOptions<SkillHubDbContext> options)
        : base(options)
    {
    }

    /// <summary>Gets the database set for Participant entities.</summary>
    public DbSet<Participant> Participants => Set<Participant>();

    /// <summary>Gets the database set for Instructor entities.</summary>
    public DbSet<Instructor> Instructors => Set<Instructor>();

    /// <summary>Gets the database set for Workshop entities.</summary>
    public DbSet<Workshop> Workshops => Set<Workshop>();

    /// <summary>Gets the database set for Enrollment entities.</summary>
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    /// <summary>
    /// Applies entity configurations from the current infrastructure assembly.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(SkillHubDbContext).Assembly);
    }
}
