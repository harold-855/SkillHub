using SkillHub.Application.DTOs.Enrollments;
using SkillHub.Application.Interfaces.Repositories;

namespace SkillHub.Application.UseCases.Enrollments.GetMyEnrollments;

/// <summary>
/// Use case retrieving the personal workshop enrollments belonging to the authenticated participant.
/// </summary>
public class GetMyEnrollmentsUseCase
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IWorkshopRepository _workshopRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetMyEnrollmentsUseCase"/> class.
    /// </summary>
    public GetMyEnrollmentsUseCase(
        IEnrollmentRepository enrollmentRepository,
        IWorkshopRepository workshopRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _workshopRepository = workshopRepository;
    }

    /// <summary>
    /// Queries all enrollments registered under the participant ID and maps them to presentation responses.
    /// </summary>
    /// <param name="participantId">The participant unique identifier.</param>
    /// <returns>A collection of the participant's enrollments with workshop schedule and status.</returns>
    public async Task<IReadOnlyCollection<EnrollmentResponse>> ExecuteAsync(
        Guid participantId)
    {
        var enrollments = await _enrollmentRepository
            .GetByParticipantIdAsync(participantId);

        var responses = new List<EnrollmentResponse>();

        foreach (var enrollment in enrollments)
        {
            var workshop = await _workshopRepository
                .GetByIdAsync(enrollment.WorkshopId);

            if (workshop is null)
            {
                continue;
            }

            responses.Add(new EnrollmentResponse
            {
                Id = enrollment.Id,
                WorkshopId = workshop.Id,
                WorkshopCode = workshop.Code,
                WorkshopTitle = workshop.Title,
                WorkshopStartDateTime = workshop.StartDateTime,
                WorkshopEndDateTime = workshop.EndDateTime,
                Status = enrollment.Status,
                EnrolledAt = enrollment.EnrolledAt,
                CancelledAt = enrollment.CancelledAt
            });
        }

        return responses;
    }
}
