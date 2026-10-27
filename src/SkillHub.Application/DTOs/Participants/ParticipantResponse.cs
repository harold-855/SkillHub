namespace SkillHub.Application.DTOs.Participants;

public class ParticipantResponse
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}