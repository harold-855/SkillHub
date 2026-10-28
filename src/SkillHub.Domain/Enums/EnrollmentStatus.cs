namespace SkillHub.Domain.Enums;

/// <summary>
/// Represents the lifecycle status of a workshop enrollment.
/// </summary>
public enum EnrollmentStatus
{
    /// <summary>
    /// The enrollment is active and consumes a seat in the workshop.
    /// </summary>
    Active = 1,

    /// <summary>
    /// The enrollment has been cancelled and releases its workshop seat.
    /// </summary>
    Cancelled = 2
}
