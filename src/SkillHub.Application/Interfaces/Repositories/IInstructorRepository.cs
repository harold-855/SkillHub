using SkillHub.Domain.Entities;

namespace SkillHub.Application.Interfaces.Repositories;

public interface IInstructorRepository
{
    Task<Instructor?> GetByIdAsync(Guid id);

    Task<Instructor?> GetByEmailAsync(string email);

    Task<bool> ExistsByEmailAsync(string email);
}