using SkillHub.Application.DTOs.Enrollments;
using SkillHub.Application.Interfaces.Repositories;

namespace SkillHub.Application.UseCases.Workshops.GetWorkshopEnrollments;

/// <summary>
/// Use case allowing authorized center employees to inspect all enrollments registered for a given workshop.
/// </summary>
public class GetWorkshopEnrollmentsUseCase
{
    private readonly IWorkshopRepository _workshopRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IParticipantRepository _participantRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetWorkshopEnrollmentsUseCase"/> class.
    /// </summary>
    public GetWorkshopEnrollmentsUseCase(
        IWorkshopRepository workshopRepository,
        IEnrollmentRepository enrollmentRepository,
        IParticipantRepository participantRepository)
    {
        _workshopRepository = workshopRepository;
        _enrollmentRepository = enrollmentRepository;
        _participantRepository = participantRepository;
    }

    /// <summary>
    /// Validates workshop existence and compiles a list of enrolled participants with contact and status details.
    /// </summary>
    /// <param name="workshopId">The workshop unique identifier.</param>
    /// <returns>A collection of workshop enrollment records.</returns>
    public async Task<IReadOnlyCollection<WorkshopEnrollmentResponse>> ExecuteAsync(
        Guid workshopId)
    {
        var workshop = await _workshopRepository.GetByIdAsync(workshopId);

        if (workshop is null)
        {
            throw new InvalidOperationException(
                "The workshop does not exist.");
        }

        var enrollments = await _enrollmentRepository
            .GetByWorkshopIdAsync(workshopId);

        var responses = new List<WorkshopEnrollmentResponse>();

        foreach (var enrollment in enrollments)
        {
            var participant = await _participantRepository
                .GetByIdAsync(enrollment.ParticipantId);

            if (participant is null)
            {
                continue;
            }

            responses.Add(new WorkshopEnrollmentResponse
            {
                EnrollmentId = enrollment.Id,
                ParticipantId = participant.Id,
                ParticipantName = participant.FullName,
                ParticipantEmail = participant.Email,
                WorkshopCode = workshop.Code,
                WorkshopTitle = workshop.Title,
                EnrolledAt = enrollment.EnrolledAt,
                Status = enrollment.Status.ToString()
            });
        }

        return responses;
    }
}
