namespace SkillHub.Application.DTOs.Participants;

/// <summary>
/// Data transfer object representing a participant summary along with their active enrollment count.
/// </summary>
public class ParticipantSummaryResponse
{
    /// <summary>The unique participant identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>The participant's full name.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>The participant's registered email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>The total number of active workshop enrollments held by this participant.</summary>
    public int ActiveEnrollmentsCount { get; set; }
}
