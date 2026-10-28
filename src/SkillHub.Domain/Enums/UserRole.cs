namespace SkillHub.Domain.Enums;

/// <summary>
/// Defines user security roles within the SkillHub system.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Student or trainee who can browse workshops and manage personal enrollments.
    /// </summary>
    Participant = 1,

    /// <summary>
    /// Staff member or instructor who publishes workshops and manages center operations.
    /// </summary>
    Employee = 2
}
