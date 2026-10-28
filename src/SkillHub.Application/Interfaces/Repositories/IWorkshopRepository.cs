using SkillHub.Domain.Entities;

namespace SkillHub.Application.Interfaces.Repositories;

/// <summary>
/// Repository contract for Workshop aggregate persistence operations.
/// </summary>
public interface IWorkshopRepository
{
    /// <summary>
    /// Retrieves a workshop by its unique identifier.
    /// </summary>
    Task<Workshop?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves a workshop by its unique uppercase code.
    /// </summary>
    Task<Workshop?> GetByCodeAsync(string code);

    /// <summary>
    /// Retrieves all future workshops that have not yet started.
    /// </summary>
    Task<IReadOnlyCollection<Workshop>> GetAvailableAsync();

    /// <summary>
    /// Retrieves all workshops registered in the system.
    /// </summary>
    Task<IReadOnlyCollection<Workshop>> GetAllAsync();

    /// <summary>
    /// Persists a new workshop entity.
    /// </summary>
    Task AddAsync(Workshop workshop);

    /// <summary>
    /// Checks whether a workshop code already exists.
    /// </summary>
    Task<bool> ExistsByCodeAsync(string code);
}
