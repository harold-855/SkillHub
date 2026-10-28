namespace SkillHub.Application.DTOs.Participants;

/// <summary>
/// Data transfer object representing participant identity details.
/// </summary>
public class ParticipantResponse
{
    /// <summary>The unique participant identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>The participant's full name.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>The participant's registered email address.</summary>
    public string Email { get; set; } = string.Empty;
}
