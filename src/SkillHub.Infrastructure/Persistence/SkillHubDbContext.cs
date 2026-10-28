using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkillHub.Domain.Entities;
using SkillHub.Infrastructure.Identity;

namespace SkillHub.Infrastructure.Persistence;

public class SkillHubDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public SkillHubDbContext(
        DbContextOptions<SkillHubDbContext> options)
        : base(options)
    {
    }

    public DbSet<Participant> Participants => Set<Participant>();

    public DbSet<Instructor> Instructors => Set<Instructor>();

    public DbSet<Workshop> Workshops => Set<Workshop>();

    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(SkillHubDbContext).Assembly);
    }
}
