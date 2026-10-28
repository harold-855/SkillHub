using SkillHub.Application.DTOs.Workshops;
using SkillHub.Application.Interfaces.Repositories;
using SkillHub.Domain.Enums;

namespace SkillHub.Application.UseCases.Reports.GetTopWorkshops;

/// <summary>
/// Use case generating analytical reports of workshops ordered by their active enrollment volume.
/// </summary>
public class GetTopWorkshopsUseCase
{
    private readonly IWorkshopRepository _workshopRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IInstructorRepository _instructorRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTopWorkshopsUseCase"/> class.
    /// </summary>
    public GetTopWorkshopsUseCase(
        IWorkshopRepository workshopRepository,
        IEnrollmentRepository enrollmentRepository,
        IInstructorRepository instructorRepository)
    {
        _workshopRepository = workshopRepository;
        _enrollmentRepository = enrollmentRepository;
        _instructorRepository = instructorRepository;
    }

    /// <summary>
    /// Queries workshops, aggregates active enrollment counts, and returns list descending by popularity.
    /// </summary>
    /// <returns>A collection of workshops ordered from highest to lowest active enrollment count.</returns>
    public async Task<IReadOnlyCollection<TopWorkshopResponse>>
        ExecuteAsync()
    {
        var workshops = await _workshopRepository.GetAllAsync();

        var report = new List<TopWorkshopResponse>();

        foreach (var workshop in workshops)
        {
            var instructor = await _instructorRepository
                .GetByIdAsync(workshop.InstructorId);

            if (instructor is null)
            {
                continue;
            }

            var enrollments = await _enrollmentRepository
                .GetByWorkshopIdAsync(workshop.Id);

            var activeEnrollments = enrollments.Count(
                enrollment => enrollment.Status == EnrollmentStatus.Active);

            report.Add(new TopWorkshopResponse
            {
                WorkshopId = workshop.Id,
                Code = workshop.Code,
                Title = workshop.Title,
                InstructorName = instructor.FullName,
                ActiveEnrollments = activeEnrollments,
                MaximumCapacity = workshop.MaximumCapacity
            });
        }

        return report
            .OrderByDescending(workshop => workshop.ActiveEnrollments)
            .ToList();
    }
}
