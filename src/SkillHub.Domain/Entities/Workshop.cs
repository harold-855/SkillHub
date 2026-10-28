using SkillHub.Domain.Shared;

namespace SkillHub.Domain.Entities;

/// <summary>
/// Represents a short in-person training workshop offered by SkillHub.
/// </summary>
public class Workshop : BaseEntity
{
    /// <summary>
    /// Gets the unique uppercase alphanumeric code identifying the workshop (e.g., FOT-001).
    /// </summary>
    public string Code { get; private set; }

    /// <summary>
    /// Gets the title of the workshop.
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Gets the detailed description of the workshop content and objectives.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Gets the foreign key identifier of the assigned instructor.
    /// </summary>
    public Guid InstructorId { get; private set; }

    /// <summary>
    /// Gets the scheduled start date and time.
    /// </summary>
    public DateTime StartDateTime { get; private set; }

    /// <summary>
    /// Gets the scheduled end date and time.
    /// </summary>
    public DateTime EndDateTime { get; private set; }

    /// <summary>
    /// Gets the maximum seating capacity configured for the workshop.
    /// </summary>
    public int MaximumCapacity { get; private set; }

    private readonly List<Enrollment> _enrollments = new();

    /// <summary>
    /// Gets the collection of enrollments associated with this workshop.
    /// </summary>
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    private Workshop()
    {
        Code = string.Empty;
        Title = string.Empty;
        Description = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Workshop"/> class, enforcing business constraints
    /// such as mandatory future start dates and end time posterior to start time.
    /// </summary>
    /// <param name="code">Unique workshop identifier code.</param>
    /// <param name="title">Workshop title.</param>
    /// <param name="description">Workshop description.</param>
    /// <param name="instructorId">Instructor identifier.</param>
    /// <param name="startDateTime">Start timestamp (must be in the future).</param>
    /// <param name="endDateTime">End timestamp (must be after startDateTime).</param>
    /// <param name="maximumCapacity">Maximum seats available (must be greater than zero).</param>
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
