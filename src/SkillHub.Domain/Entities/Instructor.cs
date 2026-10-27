using SkillHub.Domain.Shared;

namespace SkillHub.Domain.Entities;

public class Instructor : BaseEntity
{
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public string Specialty { get; private set; }

    private readonly List<Workshop> _workshops = new();

    public IReadOnlyCollection<Workshop> Workshops => _workshops.AsReadOnly();

    private Instructor()
    {
        FullName = string.Empty;
        Email = string.Empty;
        Specialty = string.Empty;
    }

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