namespace SkillHub.Application.DTOs.Workshops;

/// <summary>
/// Detailed response containing comprehensive workshop information and instructor profile details.
/// </summary>
public class WorkshopDetailsResponse
{
    /// <summary>The workshop unique identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>The unique code of the workshop.</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>The workshop title.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>The detailed content description.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>The full name of the instructor.</summary>
    public string InstructorName { get; set; } = string.Empty;

    /// <summary>The email address of the instructor.</summary>
    public string InstructorEmail { get; set; } = string.Empty;

    /// <summary>The specialty area of the instructor.</summary>
    public string InstructorSpecialty { get; set; } = string.Empty;

    /// <summary>The workshop start date and time.</summary>
    public DateTime StartDateTime { get; set; }

    /// <summary>The workshop end date and time.</summary>
    public DateTime EndDateTime { get; set; }

    /// <summary>The maximum seating capacity.</summary>
    public int MaximumCapacity { get; set; }

    /// <summary>The remaining available seats calculated from active enrollments.</summary>
    public int AvailableSeats { get; set; }
}
