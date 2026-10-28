using SkillHub.Application.DTOs.Workshops;
using SkillHub.Application.Interfaces.Repositories;
using SkillHub.Domain.Entities;

namespace SkillHub.Application.UseCases.Workshops.CreateWorkshop;

/// <summary>
/// Use case managing workshop publication by authorized center employees.
/// </summary>
public class CreateWorkshopUseCase
{
    private readonly IWorkshopRepository _workshopRepository;
    private readonly IInstructorRepository _instructorRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateWorkshopUseCase"/> class.
    /// </summary>
    public CreateWorkshopUseCase(
        IWorkshopRepository workshopRepository,
        IInstructorRepository instructorRepository)
    {
        _workshopRepository = workshopRepository;
        _instructorRepository = instructorRepository;
    }

    /// <summary>
    /// Validates instructor existence, ensures code uniqueness, and persists a new workshop aggregate.
    /// </summary>
    /// <param name="request">The creation payload.</param>
    /// <returns>The unique identifier of the newly created workshop.</returns>
    public async Task<Guid> ExecuteAsync(CreateWorkshopRequest request)
    {
        var instructor = await _instructorRepository
            .GetByEmailAsync(request.InstructorEmail);

        if (instructor is null)
        {
            throw new InvalidOperationException(
                "The specified instructor does not exist.");
        }

        var workshopCode = request.Code.Trim().ToUpperInvariant();

        var codeAlreadyExists = await _workshopRepository
            .ExistsByCodeAsync(workshopCode);

        if (codeAlreadyExists)
        {
            throw new InvalidOperationException(
                "A workshop with this code already exists.");
        }

        var workshop = new Workshop(
            request.Code,
            request.Title,
            request.Description,
            instructor.Id,
            request.StartDateTime,
            request.EndDateTime,
            request.MaximumCapacity);

        await _workshopRepository.AddAsync(workshop);

        return workshop.Id;
    }
}
