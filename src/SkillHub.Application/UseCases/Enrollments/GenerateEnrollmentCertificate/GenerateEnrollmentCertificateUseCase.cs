using SkillHub.Application.DTOs.Certificates;
using SkillHub.Application.Interfaces.Documents;
using SkillHub.Application.Interfaces.Repositories;
using SkillHub.Domain.Enums;

namespace SkillHub.Application.UseCases.Enrollments.GenerateEnrollmentCertificate;

public class GenerateEnrollmentCertificateUseCase
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IParticipantRepository _participantRepository;
    private readonly IWorkshopRepository _workshopRepository;
    private readonly IInstructorRepository _instructorRepository;
    private readonly ICertificateGenerator _certificateGenerator;

    public GenerateEnrollmentCertificateUseCase(
        IEnrollmentRepository enrollmentRepository,
        IParticipantRepository participantRepository,
        IWorkshopRepository workshopRepository,
        IInstructorRepository instructorRepository,
        ICertificateGenerator certificateGenerator)
    {
        _enrollmentRepository = enrollmentRepository;
        _participantRepository = participantRepository;
        _workshopRepository = workshopRepository;
        _instructorRepository = instructorRepository;
        _certificateGenerator = certificateGenerator;
    }

    public async Task<byte[]> ExecuteAsync(
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

        if (enrollment.Status != EnrollmentStatus.Active)
        {
            throw new InvalidOperationException(
                "A certificate can only be generated for an active enrollment.");
        }

        if (!isEmployee && enrollment.ParticipantId != currentUserId)
        {
            throw new UnauthorizedAccessException(
                "You can only download certificates for your own enrollments.");
        }

        var participant = await _participantRepository
            .GetByIdAsync(enrollment.ParticipantId);

        if (participant is null)
        {
            throw new InvalidOperationException(
                "The participant associated with the enrollment does not exist.");
        }

        var workshop = await _workshopRepository
            .GetByIdAsync(enrollment.WorkshopId);

        if (workshop is null)
        {
            throw new InvalidOperationException(
                "The workshop associated with the enrollment does not exist.");
        }

        var instructor = await _instructorRepository
            .GetByIdAsync(workshop.InstructorId);

        if (instructor is null)
        {
            throw new InvalidOperationException(
                "The instructor associated with the workshop does not exist.");
        }

        var certificateData = new EnrollmentCertificateData
        {
            WorkshopTitle = workshop.Title,
            WorkshopCode = workshop.Code,
            ParticipantName = participant.FullName,
            ParticipantEmail = participant.Email,
            InstructorName = instructor.FullName,
            WorkshopStartDateTime = workshop.StartDateTime,
            WorkshopEndDateTime = workshop.EndDateTime,
            EnrolledAt = enrollment.EnrolledAt,
            GeneratedAt = DateTime.UtcNow
        };

        return await _certificateGenerator.GenerateAsync(certificateData);
    }
}