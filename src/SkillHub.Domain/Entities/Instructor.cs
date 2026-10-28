using SkillHub.Domain.Shared;

namespace SkillHub.Domain.Entities;

/// <summary>
/// Represents an instructor employed by SkillHub who teaches workshops.
/// </summary>
public class Instructor : BaseEntity
{
    /// <summary>
    /// Gets the full name of the instructor.
    /// </summary>
    public string FullName { get; private set; }

    /// <summary>
    /// Gets the email address of the instructor.
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    /// Gets the professional specialty or teaching domain.
    /// </summary>
    public string Specialty { get; private set; }

    private readonly List<Workshop> _workshops = new();

    /// <summary>
    /// Gets the collection of workshops taught by this instructor.
    /// </summary>
    public IReadOnlyCollection<Workshop> Workshops => _workshops.AsReadOnly();

    private Instructor()
    {
        FullName = string.Empty;
        Email = string.Empty;
        Specialty = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Instructor"/> class with domain validations.
    /// </summary>
    /// <param name="fullName">The instructor's full name.</param>
    /// <param name="email">The instructor's corporate email.</param>
    /// <param name="specialty">The instructor's field of expertise.</param>
    public Instructor(
        string fullName,
        string email,
        string specialty)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.", nameof(fullName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        if (string.IsNullOrWhiteSpace(specialty))
            throw new ArgumentException("Specialty is required.", nameof(specialty));

        FullName = fullName.Trim();
        Email = email.Trim().ToLowerInvariant();
        Specialty = specialty.Trim();
    }
}
