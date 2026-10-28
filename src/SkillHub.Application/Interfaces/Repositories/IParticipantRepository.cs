using SkillHub.Domain.Entities;

namespace SkillHub.Application.Interfaces.Repositories;

/// <summary>
/// Repository contract for Participant aggregate persistence.
/// </summary>
public interface IParticipantRepository
{
    /// <summary>
    /// Retrieves a participant by unique identifier.
    /// </summary>
    Task<Participant?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves a participant by normalized email address.
    /// </summary>
    Task<Participant?> GetByEmailAsync(string email);

    /// <summary>
    /// Adds a new participant to the repository.
    /// </summary>
    Task AddAsync(Participant participant);

    /// <summary>
    /// Determines whether a participant with the specified email already exists.
    /// </summary>
    Task<bool> ExistsByEmailAsync(string email);
}
