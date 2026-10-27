using SkillHub.Domain.Enums;

namespace SkillHub.Application.DTOs.Enrollments;

public class CreateEnrollmentRequest
{
    public Guid WorkshopId { get; set; }
}