using SkillHub.Domain.Shared;

namespace SkillHub.Domain.Entities;

/// <summary>
/// Represents a participant registered in the SkillHub platform.
/// </summary>
public class Participant : BaseEntity
{
    /// <summary>
    /// Gets the full legal name of the participant.
    /// </summary>
    public string FullName { get; private set; }

    /// <summary>
    /// Gets the normalized contact and login email address.
    /// </summary>
    public string Email { get; private set; }

    private readonly List<Enrollment> _enrollments = new();

    /// <summary>
    /// Gets the collection of enrollments associated with this participant.
    /// </summary>
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    private Participant()
    {
        FullName = string.Empty;
        Email = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Participant"/> class with required domain validations.
    /// </summary>
    /// <param name="fullName">The participant's full name.</param>
    /// <param name="email">The participant's email address.</param>
    public Participant(string fullName, string email)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.", nameof(fullName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        FullName = fullName.Trim();
        Email = email.Trim().ToLowerInvariant();
    }
}
