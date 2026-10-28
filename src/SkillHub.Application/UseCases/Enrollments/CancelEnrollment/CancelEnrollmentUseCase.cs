using SkillHub.Application.Interfaces.Repositories;
using SkillHub.Domain.Enums;

namespace SkillHub.Application.UseCases.Enrollments.CancelEnrollment;

/// <summary>
/// Use case managing enrollment cancellation.
/// Enforces business rules: participants can only cancel their own enrollments up to 24 hours
/// prior to workshop start; employees can cancel any enrollment as long as the workshop hasn't started yet.
/// </summary>
public class CancelEnrollmentUseCase
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IWorkshopRepository _workshopRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CancelEnrollmentUseCase"/> class.
    /// </summary>
    public CancelEnrollmentUseCase(
        IEnrollmentRepository enrollmentRepository,
        IWorkshopRepository workshopRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _workshopRepository = workshopRepository;
    }

    /// <summary>
    /// Validates cancellation permissions and timing constraints, then transitions enrollment to Cancelled status.
    /// </summary>
    /// <param name="enrollmentId">The enrollment unique identifier.</param>
    /// <param name="currentUserId">The user identifier initiating cancellation.</param>
    /// <param name="isEmployee">Flag indicating whether the user has administrative employee privileges.</param>
    public async Task ExecuteAsync(
        Guid enrollmentId,
        Guid currentUserId,
        bool isEmployee)
    {
        var enrollment = await _enrollmentRepository
            .GetByIdAsync(enrollmentId);

        if (enrollment is null)
        {
            throw new InvalidOperationException(
                "The enrollment does not exist.");
        }

        if (!isEmployee && enrollment.ParticipantId != currentUserId)
        {
            throw new UnauthorizedAccessException(
                "You can only cancel your own enrollments.");
        }

        var workshop = await _workshopRepository
            .GetByIdAsync(enrollment.WorkshopId);

        if (workshop is null)
        {
            throw new InvalidOperationException(
                "The workshop does not exist.");
        }

        if (workshop.StartDateTime <= DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "The enrollment cannot be cancelled because the workshop has already started.");
        }

        if (!isEmployee)
        {
            var cancellationDeadline = workshop.StartDateTime
                .Subtract(TimeSpan.FromHours(24));

            if (DateTime.UtcNow > cancellationDeadline)
            {
                throw new InvalidOperationException(
                    "Participants must cancel at least 24 hours before the workshop starts.");
            }
        }

        if (enrollment.Status == EnrollmentStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "The enrollment is already cancelled.");
        }

        enrollment.Cancel(currentUserId);

        await _enrollmentRepository.UpdateAsync(enrollment);
    }
}
