namespace SkillHub.Domain.Shared;

/// <summary>
/// Base entity class providing a unique identifier for all domain entities.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    public Guid Id { get; protected set; } = Guid.NewGuid();
}
