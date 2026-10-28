using SkillHub.Application.DTOs.Workshops;
using SkillHub.Application.Interfaces.Repositories;

namespace SkillHub.Application.UseCases.Workshops.GetAvailableWorkshops;

/// <summary>
/// Use case retrieving future workshops available in the public catalog with real-time seat availability.
/// </summary>
public class GetAvailableWorkshopsUseCase
{
    private readonly IWorkshopRepository _workshopRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IInstructorRepository _instructorRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAvailableWorkshopsUseCase"/> class.
    /// </summary>
    public GetAvailableWorkshopsUseCase(
        IWorkshopRepository workshopRepository,
        IEnrollmentRepository enrollmentRepository,
        IInstructorRepository instructorRepository)
    {
        _workshopRepository = workshopRepository;
        _enrollmentRepository = enrollmentRepository;
        _instructorRepository = instructorRepository;
    }

    /// <summary>
    /// Fetches all workshops starting in the future and computes their available seats.
    /// </summary>
    /// <returns>A collection of workshops with instructor and seat availability details.</returns>
    public async Task<IReadOnlyCollection<WorkshopResponse>> ExecuteAsync()
    {
        var workshops = await _workshopRepository.GetAvailableAsync();

        var responses = new List<WorkshopResponse>();

        foreach (var workshop in workshops)
        {
            var instructor = await _instructorRepository
                .GetByIdAsync(workshop.InstructorId);

            if (instructor is null)
            {
                continue;
            }

            var activeEnrollments = await _enrollmentRepository
                .CountActiveByWorkshopIdAsync(workshop.Id);

            var availableSeats = Math.Max(
                0,
                workshop.MaximumCapacity - activeEnrollments);

            responses.Add(new WorkshopResponse
            {
                Id = workshop.Id,
                Code = workshop.Code,
                Title = workshop.Title,
                Description = workshop.Description,
                InstructorName = instructor.FullName,
                StartDateTime = workshop.StartDateTime,
                EndDateTime = workshop.EndDateTime,
                MaximumCapacity = workshop.MaximumCapacity,
                AvailableSeats = availableSeats
            });
        }

        return responses;
    }
}
