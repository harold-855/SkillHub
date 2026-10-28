using SkillHub.Domain.Entities;

namespace SkillHub.Application.Interfaces.Repositories;

/// <summary>
/// Repository contract for Instructor aggregate persistence and lookups.
/// </summary>
public interface IInstructorRepository
{
    /// <summary>
    /// Retrieves an instructor by unique identifier.
    /// </summary>
    Task<Instructor?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves an instructor by corporate email address.
    /// </summary>
    Task<Instructor?> GetByEmailAsync(string email);

    /// <summary>
    /// Verifies whether an instructor exists with the given email address.
    /// </summary>
    Task<bool> ExistsByEmailAsync(string email);
}
