using SkillHub.Domain.Shared;

namespace SkillHub.Domain.Entities;

public class Participant : BaseEntity
{
    public string FullName { get; private set; }
    public string Email { get; private set; }

    private readonly List<Enrollment> _enrollments = new();

    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    private Participant()
    {
        FullName = string.Empty;
        Email = string.Empty;
    }

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