using SkillHub.Application.DTOs.Enrollments;
using SkillHub.Application.Interfaces.Repositories;
using SkillHub.Domain.Entities;

namespace SkillHub.Application.UseCases.Enrollments.CreateEnrollment;

public class CreateEnrollmentUseCase
{
    private readonly IParticipantRepository _participantRepository;
    private readonly IWorkshopRepository _workshopRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public CreateEnrollmentUseCase(
        IParticipantRepository participantRepository,
        IWorkshopRepository workshopRepository,
        IEnrollmentRepository enrollmentRepository)
    {
        _participantRepository = participantRepository;
        _workshopRepository = workshopRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<Guid> ExecuteAsync(
        Guid participantId,
        CreateEnrollmentRequest request)
    {
        var participant = await _participantRepository
            .GetByIdAsync(participantId);

        if (participant is null)
        {
            throw new InvalidOperationException(
                "The participant does not exist.");
        }

        var workshop = await _workshopRepository
            .GetByIdAsync(request.WorkshopId);

        if (workshop is null)
        {
            throw new InvalidOperationException(
                "The workshop does not exist.");
        }

        if (workshop.StartDateTime <= DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "Enrollment is not allowed because the workshop has already started.");
        }

        var activeEnrollmentExists = await _enrollmentRepository
            .HasActiveEnrollmentAsync(participantId, workshop.Id);

        if (activeEnrollmentExists)
        {
            throw new InvalidOperationException(
                "The participant already has an active enrollment for this workshop.");
        }

        var activeEnrollments = await _enrollmentRepository
            .CountActiveByWorkshopIdAsync(workshop.Id);

        if (activeEnrollments >= workshop.MaximumCapacity)
        {
            throw new InvalidOperationException(
                "The workshop has no available seats.");
        }

        var hasScheduleConflict = await _enrollmentRepository
            .HasScheduleConflictAsync(
                participantId,
                workshop.StartDateTime,
                workshop.EndDateTime);

        if (hasScheduleConflict)
        {
            throw new InvalidOperationException(
                "The workshop schedule conflicts with another active enrollment.");
        }

        var enrollment = new Enrollment(
            participantId,
            workshop.Id);

        await _enrollmentRepository.AddAsync(enrollment);

        return enrollment.Id;
    }
}