using SkillHub.Domain.Shared;

namespace SkillHub.Domain.Entities;

public class Workshop : BaseEntity
{
    public string Code { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }

    public Guid InstructorId { get; private set; }
    public DateTime StartDateTime { get; private set; }
    public DateTime EndDateTime { get; private set; }
    public int MaximumCapacity { get; private set; }

    private readonly List<Enrollment> _enrollments = new();

    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    private Workshop()
    {
        Code = string.Empty;
        Title = string.Empty;
        Description = string.Empty;
    }

    public Workshop(
        string code,
        string title,
        string description,
        Guid instructorId,
        DateTime startDateTime,
        DateTime endDateTime,
        int maximumCapacity)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Workshop code is required.", nameof(code));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Workshop title is required.", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Workshop description is required.", nameof(description));

        if (instructorId == Guid.Empty)
            throw new ArgumentException("Instructor is required.", nameof(instructorId));

        if (startDateTime <= DateTime.UtcNow)
            throw new ArgumentException(
                "Workshop must have a future start date.",
                nameof(startDateTime));

        if (endDateTime <= startDateTime)
            throw new ArgumentException(
                "Workshop end date must be later than start date.",
                nameof(endDateTime));

        if (maximumCapacity <= 0)
            throw new ArgumentException(
                "Maximum capacity must be greater than zero.",
                nameof(maximumCapacity));

        Code = code.Trim().ToUpperInvariant();
        Title = title.Trim();
        Description = description.Trim();
        InstructorId = instructorId;
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        MaximumCapacity = maximumCapacity;
    }
}