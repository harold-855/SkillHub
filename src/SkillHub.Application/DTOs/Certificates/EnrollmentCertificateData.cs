namespace SkillHub.Application.DTOs.Certificates;

/// <summary>
/// Data container providing metadata required for generating an official SkillHub enrollment certificate.
/// </summary>
public class EnrollmentCertificateData
{
    /// <summary>The title of the workshop.</summary>
    public string WorkshopTitle { get; set; } = string.Empty;

    /// <summary>The unique code of the workshop (e.g., FOT-001).</summary>
    public string WorkshopCode { get; set; } = string.Empty;

    /// <summary>The full name of the enrolled participant.</summary>
    public string ParticipantName { get; set; } = string.Empty;

    /// <summary>The email address of the enrolled participant.</summary>
    public string ParticipantEmail { get; set; } = string.Empty;

    /// <summary>The full name of the assigned instructor.</summary>
    public string InstructorName { get; set; } = string.Empty;

    /// <summary>The scheduled start date and time of the workshop.</summary>
    public DateTime WorkshopStartDateTime { get; set; }

    /// <summary>The scheduled end date and time of the workshop.</summary>
    public DateTime WorkshopEndDateTime { get; set; }

    /// <summary>The timestamp when the participant enrolled.</summary>
    public DateTime EnrolledAt { get; set; }

    /// <summary>The timestamp when the certificate document was generated.</summary>
    public DateTime GeneratedAt { get; set; }
}
