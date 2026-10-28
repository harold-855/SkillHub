using SkillHub.Domain.Enums;
using SkillHub.Domain.Shared;

namespace SkillHub.Domain.Entities;

/// <summary>
/// Represents an enrollment connecting a participant to a workshop.
/// </summary>
public class Enrollment : BaseEntity
{
    /// <summary>
    /// Gets the foreign key identifier of the enrolled participant.
    /// </summary>
    public Guid ParticipantId { get; private set; }

    /// <summary>
    /// Gets the foreign key identifier of the workshop.
    /// </summary>
    public Guid WorkshopId { get; private set; }

    /// <summary>
    /// Gets the current status of the enrollment (Active or Cancelled).
    /// </summary>
    public EnrollmentStatus Status { get; private set; }

    /// <summary>
    /// Gets the timestamp when the enrollment was created.
    /// </summary>
    public DateTime EnrolledAt { get; private set; }

    /// <summary>
    /// Gets the identifier of the user (participant or employee) who cancelled the enrollment, if applicable.
    /// </summary>
    public Guid? CancelledByUserId { get; private set; }

    /// <summary>
    /// Gets the timestamp when cancellation occurred, if applicable.
    /// </summary>
    public DateTime? CancelledAt { get; private set; }

    private Enrollment()
    {
    }

    /// <summary>
    /// Initializes a new active enrollment instance.
    /// </summary>
    /// <param name="participantId">The participant identifier.</param>
    /// <param name="workshopId">The workshop identifier.</param>
    public Enrollment(
        Guid participantId,
        Guid workshopId)
    {
        if (participantId == Guid.Empty)
            throw new ArgumentException(
                "Participant is required.",
                nameof(participantId));

        if (workshopId == Guid.Empty)
            throw new ArgumentException(
                "Workshop is required.",
                nameof(workshopId));

        ParticipantId = participantId;
        WorkshopId = workshopId;
        Status = EnrollmentStatus.Active;
        EnrolledAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels the enrollment, recording who performed the cancellation and the current timestamp.
    /// </summary>
    /// <param name="cancelledByUserId">The identifier of the user cancelling the enrollment.</param>
    public void Cancel(Guid cancelledByUserId)
    {
        if (cancelledByUserId == Guid.Empty)
            throw new ArgumentException(
                "The user who cancels the enrollment is required.",
                nameof(cancelledByUserId));

        if (Status == EnrollmentStatus.Cancelled)
            throw new InvalidOperationException(
                "Enrollment is already cancelled.");

        Status = EnrollmentStatus.Cancelled;
        CancelledByUserId = cancelledByUserId;
        CancelledAt = DateTime.UtcNow;
    }
}
