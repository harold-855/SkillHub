namespace SkillHub.Application.DTOs.Workshops;

public class WorkshopResponse
{
    public Guid Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string InstructorName { get; set; } = string.Empty;

    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    public int MaximumCapacity { get; set; }

    public int AvailableSeats { get; set; }
}