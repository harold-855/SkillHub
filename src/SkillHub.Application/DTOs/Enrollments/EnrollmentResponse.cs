using SkillHub.Domain.Enums;

namespace SkillHub.Application.DTOs.Enrollments;

public class EnrollmentResponse
{
    public Guid Id { get; set; }

    public Guid WorkshopId { get; set; }

    public string WorkshopCode { get; set; } = string.Empty;

    public string WorkshopTitle { get; set; } = string.Empty;

    public DateTime WorkshopStartDateTime { get; set; }

    public DateTime WorkshopEndDateTime { get; set; }

    public EnrollmentStatus Status { get; set; }

    public DateTime EnrolledAt { get; set; }

    public DateTime? CancelledAt { get; set; }
}