using Microsoft.EntityFrameworkCore;
using SkillHub.Application.Interfaces.Repositories;
using SkillHub.Domain.Entities;

namespace SkillHub.Infrastructure.Persistence.Repositories;

public class WorkshopRepository : IWorkshopRepository
{
    private readonly SkillHubDbContext _dbContext;

    public WorkshopRepository(SkillHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Workshop?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Workshops
            .FirstOrDefaultAsync(workshop => workshop.Id == id);
    }

    public async Task<Workshop?> GetByCodeAsync(string code)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();

        return await _dbContext.Workshops
            .FirstOrDefaultAsync(workshop => workshop.Code == normalizedCode);
    }

    public async Task<IReadOnlyCollection<Workshop>> GetAvailableAsync()
    {
        var currentDateTime = DateTime.UtcNow;

        return await _dbContext.Workshops
            .Where(workshop => workshop.StartDateTime > currentDateTime)
            .OrderBy(workshop => workshop.StartDateTime)
            .ToListAsync();
    }

    public async Task<IReadOnlyCollection<Workshop>> GetAllAsync()
    {
        return await _dbContext.Workshops
            .OrderBy(workshop => workshop.StartDateTime)
            .ToListAsync();
    }

    public async Task AddAsync(Workshop workshop)
    {
        await _dbContext.Workshops.AddAsync(workshop);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> ExistsByCodeAsync(string code)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();

        return await _dbContext.Workshops
            .AnyAsync(workshop => workshop.Code == normalizedCode);
    }
}