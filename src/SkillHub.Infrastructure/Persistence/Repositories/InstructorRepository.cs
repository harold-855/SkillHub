using Microsoft.EntityFrameworkCore;
using SkillHub.Application.Interfaces.Repositories;
using SkillHub.Domain.Entities;

namespace SkillHub.Infrastructure.Persistence.Repositories;

public class InstructorRepository : IInstructorRepository
{
    private readonly SkillHubDbContext _dbContext;

    public InstructorRepository(SkillHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Instructor?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Instructors
            .FirstOrDefaultAsync(instructor => instructor.Id == id);
    }

    public async Task<Instructor?> GetByEmailAsync(string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return await _dbContext.Instructors
            .FirstOrDefaultAsync(
                instructor => instructor.Email == normalizedEmail);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return await _dbContext.Instructors
            .AnyAsync(instructor => instructor.Email == normalizedEmail);
    }
}