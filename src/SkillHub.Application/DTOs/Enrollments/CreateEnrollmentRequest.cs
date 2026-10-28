namespace SkillHub.Application.DTOs.Enrollments;

/// <summary>
/// Request payload submitted by a participant to enroll in a specific workshop.
/// </summary>
public class CreateEnrollmentRequest
{
    /// <summary>The unique identifier of the target workshop.</summary>
    public Guid WorkshopId { get; set; }
}
