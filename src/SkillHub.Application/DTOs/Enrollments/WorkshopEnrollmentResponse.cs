namespace SkillHub.Application.DTOs.Enrollments;

public class WorkshopEnrollmentResponse
{
    public Guid EnrollmentId { get; set; }

    public Guid ParticipantId { get; set; }

    public string ParticipantName { get; set; } = string.Empty;

    public string ParticipantEmail { get; set; } = string.Empty;

    public string WorkshopCode { get; set; } = string.Empty;

    public string WorkshopTitle { get; set; } = string.Empty;

    public DateTime EnrolledAt { get; set; }

    public string Status { get; set; } = string.Empty;
}