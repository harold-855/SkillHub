namespace SkillHub.Application.DTOs.Certificates;

public class EnrollmentCertificateData
{
    public string WorkshopTitle { get; set; } = string.Empty;

    public string WorkshopCode { get; set; } = string.Empty;

    public string ParticipantName { get; set; } = string.Empty;

    public string ParticipantEmail { get; set; } = string.Empty;

    public string InstructorName { get; set; } = string.Empty;

    public DateTime WorkshopStartDateTime { get; set; }

    public DateTime WorkshopEndDateTime { get; set; }

    public DateTime EnrolledAt { get; set; }

    public DateTime GeneratedAt { get; set; }
}