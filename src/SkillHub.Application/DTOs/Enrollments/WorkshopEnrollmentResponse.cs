namespace SkillHub.Application.DTOs.Enrollments;

/// <summary>
/// Data transfer object representing an enrollment entry from the perspective of an employee managing a workshop.
/// </summary>
public class WorkshopEnrollmentResponse
{
    /// <summary>The enrollment unique identifier.</summary>
    public Guid EnrollmentId { get; set; }

    /// <summary>The participant unique identifier.</summary>
    public Guid ParticipantId { get; set; }

    /// <summary>The full name of the enrolled participant.</summary>
    public string ParticipantName { get; set; } = string.Empty;

    /// <summary>The email address of the enrolled participant.</summary>
    public string ParticipantEmail { get; set; } = string.Empty;

    /// <summary>The workshop unique code.</summary>
    public string WorkshopCode { get; set; } = string.Empty;

    /// <summary>The workshop title.</summary>
    public string WorkshopTitle { get; set; } = string.Empty;

    /// <summary>The enrollment timestamp.</summary>
    public DateTime EnrolledAt { get; set; }

    /// <summary>The textual representation of the enrollment status (Active or Cancelled).</summary>
    public string Status { get; set; } = string.Empty;
}
