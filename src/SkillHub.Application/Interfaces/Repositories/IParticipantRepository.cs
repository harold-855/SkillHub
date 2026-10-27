using SkillHub.Domain.Entities;

namespace SkillHub.Application.Interfaces.Repositories;

public interface IParticipantRepository
{
    Task<Participant?> GetByIdAsync(Guid id);

    Task<Participant?> GetByEmailAsync(string email);

    Task AddAsync(Participant participant);

    Task<bool> ExistsByEmailAsync(string email);
}