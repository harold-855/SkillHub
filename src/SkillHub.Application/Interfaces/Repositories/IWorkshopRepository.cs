using SkillHub.Domain.Entities;

namespace SkillHub.Application.Interfaces.Repositories;

public interface IWorkshopRepository
{
    Task<Workshop?> GetByIdAsync(Guid id);

    Task<Workshop?> GetByCodeAsync(string code);

    Task<IReadOnlyCollection<Workshop>> GetAvailableAsync();

    Task<IReadOnlyCollection<Workshop>> GetAllAsync();

    Task AddAsync(Workshop workshop);

    Task<bool> ExistsByCodeAsync(string code);
}