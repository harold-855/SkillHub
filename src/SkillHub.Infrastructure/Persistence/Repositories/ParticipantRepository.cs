using Microsoft.EntityFrameworkCore;
using SkillHub.Application.Interfaces.Repositories;
using SkillHub.Domain.Entities;
using SkillHub.Infrastructure.Persistence;

namespace SkillHub.Infrastructure.Persistence.Repositories;

public class ParticipantRepository : IParticipantRepository
{
    private readonly SkillHubDbContext _dbContext;

    public ParticipantRepository(SkillHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Participant?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Participants
            .FirstOrDefaultAsync(participant => participant.Id == id);
    }

    public async Task<Participant?> GetByEmailAsync(string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return await _dbContext.Participants
            .FirstOrDefaultAsync(
                participant => participant.Email == normalizedEmail);
    }

    public async Task AddAsync(Participant participant)
    {
        await _dbContext.Participants.AddAsync(participant);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return await _dbContext.Participants
            .AnyAsync(participant => participant.Email == normalizedEmail);
    }
}