using SkillHub.Application.DTOs.Workshops;
using SkillHub.Application.Interfaces.Repositories;

namespace SkillHub.Application.UseCases.Workshops.GetWorkshopById;

/// <summary>
/// Use case retrieving comprehensive details for a specific workshop, including instructor contact and available seats.
/// </summary>
public class GetWorkshopByIdUseCase
{
    private readonly IWorkshopRepository _workshopRepository;
    private readonly IInstructorRepository _instructorRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetWorkshopByIdUseCase"/> class.
    /// </summary>
    public GetWorkshopByIdUseCase(
        IWorkshopRepository workshopRepository,
        IInstructorRepository instructorRepository,
        IEnrollmentRepository enrollmentRepository)
    {
        _workshopRepository = workshopRepository;
        _instructorRepository = instructorRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    /// <summary>
    /// Finds a workshop by GUID, retrieves instructor metadata, and calculates current seat availability.
    /// </summary>
    /// <param name="id">The workshop unique identifier.</param>
    /// <returns>The workshop details response, or null if not found.</returns>
    public async Task<WorkshopDetailsResponse?> ExecuteAsync(Guid id)
    {
        var workshop = await _workshopRepository.GetByIdAsync(id);

        if (workshop is null)
        {
            return null;
        }

        var instructor = await _instructorRepository
            .GetByIdAsync(workshop.InstructorId);

        if (instructor is null)
        {
            return null;
        }

        var activeEnrollments = await _enrollmentRepository
            .CountActiveByWorkshopIdAsync(workshop.Id);

        var availableSeats = Math.Max(
            0,
            workshop.MaximumCapacity - activeEnrollments);

        return new WorkshopDetailsResponse
        {
            Id = workshop.Id,
            Code = workshop.Code,
            Title = workshop.Title,
            Description = workshop.Description,
            InstructorName = instructor.FullName,
            InstructorEmail = instructor.Email,
            InstructorSpecialty = instructor.Specialty,
            StartDateTime = workshop.StartDateTime,
            EndDateTime = workshop.EndDateTime,
            MaximumCapacity = workshop.MaximumCapacity,
            AvailableSeats = availableSeats
        };
    }
}
