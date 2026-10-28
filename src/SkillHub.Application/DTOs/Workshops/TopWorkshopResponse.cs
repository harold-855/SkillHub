namespace SkillHub.Application.DTOs.Workshops;

/// <summary>
/// Report DTO representing a workshop ordered by its active enrollment count.
/// </summary>
public class TopWorkshopResponse
{
    /// <summary>The workshop unique identifier.</summary>
    public Guid WorkshopId { get; set; }

    /// <summary>The unique workshop code.</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>The workshop title.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>The assigned instructor's name.</summary>
    public string InstructorName { get; set; } = string.Empty;

    /// <summary>The current count of active enrollments for this workshop.</summary>
    public int ActiveEnrollments { get; set; }

    /// <summary>The maximum seating capacity configured for this workshop.</summary>
    public int MaximumCapacity { get; set; }
}
