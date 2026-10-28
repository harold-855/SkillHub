namespace SkillHub.Application.DTOs.Workshops;

/// <summary>
/// Request payload submitted by center employees to create a new workshop.
/// </summary>
public class CreateWorkshopRequest
{
    /// <summary>The unique uppercase alphanumeric code for the workshop (e.g., FOT-001).</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>The title of the workshop.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>The description of the workshop curriculum.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>The email address of an existing instructor registered in the platform.</summary>
    public string InstructorEmail { get; set; } = string.Empty;

    /// <summary>The scheduled start date and time (must be in the future).</summary>
    public DateTime StartDateTime { get; set; }

    /// <summary>The scheduled end date and time (must be after StartDateTime).</summary>
    public DateTime EndDateTime { get; set; }

    /// <summary>The maximum seating capacity (must be greater than zero).</summary>
    public int MaximumCapacity { get; set; }
}
