using SkillHub.Application.DTOs.Enrollments;
using SkillHub.Application.Interfaces.Repositories;
using SkillHub.Domain.Entities;

namespace SkillHub.Application.UseCases.Enrollments.GetMyEnrollments;

public class GetMyEnrollmentsUseCase
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IWorkshopRepository _workshopRepository;

    public GetMyEnrollmentsUseCase(
        IEnrollmentRepository enrollmentRepository,
        IWorkshopRepository workshopRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _workshopRepository = workshopRepository;
    }

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