using SkillHub.Domain.Enums;

namespace SkillHub.Application.DTOs.Enrollments;

/// <summary>
/// Data transfer object representing an enrollment entry returned to the participant who owns it.
/// </summary>
public class EnrollmentResponse
{
    /// <summary>The enrollment unique identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>The workshop unique identifier.</summary>
    public Guid WorkshopId { get; set; }

    /// <summary>The workshop unique alphanumeric code (e.g., FOT-001).</summary>
    public string WorkshopCode { get; set; } = string.Empty;

    /// <summary>The title of the workshop.</summary>
    public string WorkshopTitle { get; set; } = string.Empty;

    /// <summary>The scheduled start date and time of the workshop.</summary>
    public DateTime WorkshopStartDateTime { get; set; }

    /// <summary>The scheduled end date and time of the workshop.</summary>
    public DateTime WorkshopEndDateTime { get; set; }

    /// <summary>The current status of the enrollment (Active or Cancelled).</summary>
    public EnrollmentStatus Status { get; set; }

    /// <summary>The timestamp when the participant enrolled in the workshop.</summary>
    public DateTime EnrolledAt { get; set; }

    /// <summary>The timestamp when the enrollment was cancelled, if applicable.</summary>
    public DateTime? CancelledAt { get; set; }
}
