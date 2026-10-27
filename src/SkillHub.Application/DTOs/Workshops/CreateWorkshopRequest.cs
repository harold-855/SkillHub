namespace SkillHub.Application.DTOs.Workshops;

public class CreateWorkshopRequest
{
    public string Code { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string InstructorEmail { get; set; } = string.Empty;

    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    public int MaximumCapacity { get; set; }
}