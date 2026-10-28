namespace SkillHub.Application.DTOs.Workshops;

/// <summary>
/// Data transfer object representing a workshop in the public catalog.
/// </summary>
public class WorkshopResponse
{
    /// <summary>The workshop unique identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>The unique uppercase alphanumeric code (e.g., FOT-001).</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>The title of the workshop.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>The description of what is taught in the workshop.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>The full name of the instructor teaching the workshop.</summary>
    public string InstructorName { get; set; } = string.Empty;

    /// <summary>The start timestamp of the workshop.</summary>
    public DateTime StartDateTime { get; set; }

    /// <summary>The end timestamp of the workshop.</summary>
    public DateTime EndDateTime { get; set; }

    /// <summary>The maximum capacity configured for the workshop.</summary>
    public int MaximumCapacity { get; set; }

    /// <summary>The current remaining seats available, dynamically calculated by subtracting active enrollments.</summary>
    public int AvailableSeats { get; set; }
}
