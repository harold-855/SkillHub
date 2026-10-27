namespace SkillHub.Application.DTOs.Workshops;

public class TopWorkshopResponse
{
    public Guid WorkshopId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string InstructorName { get; set; } = string.Empty;

    public int ActiveEnrollments { get; set; }

    public int MaximumCapacity { get; set; }
}