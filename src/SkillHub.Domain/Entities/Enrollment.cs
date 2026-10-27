using SkillHub.Domain.Enums;
using SkillHub.Domain.Shared;

namespace SkillHub.Domain.Entities;

public class Enrollment : BaseEntity
{
    public Guid ParticipantId { get; private set; }
    public Guid WorkshopId { get; private set; }

    public EnrollmentStatus Status { get; private set; }

    public DateTime EnrolledAt { get; private set; }

    public Guid? CancelledByUserId { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    private Enrollment()
    {
    }

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