using SkillHub.Domain.Entities;

namespace SkillHub.Application.Interfaces.Repositories;

/// <summary>
/// Repository contract for managing Enrollment entity persistence and business constraint checks.
/// </summary>
public interface IEnrollmentRepository
{
    /// <summary>
    /// Retrieves an enrollment by its unique identifier.
    /// </summary>
    Task<Enrollment?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all enrollments belonging to a specific participant.
    /// </summary>
    Task<IReadOnlyCollection<Enrollment>> GetByParticipantIdAsync(
        Guid participantId);

    /// <summary>
    /// Retrieves all enrollments registered for a specific workshop.
    /// </summary>
    Task<IReadOnlyCollection<Enrollment>> GetByWorkshopIdAsync(
        Guid workshopId);

    /// <summary>
    /// Checks whether a participant currently has an active enrollment in the specified workshop.
    /// </summary>
    Task<bool> HasActiveEnrollmentAsync(
        Guid participantId,
        Guid workshopId);

    /// <summary>
    /// Checks whether a participant has any active enrollment whose workshop schedule conflicts with the provided time range.
    /// </summary>
    Task<bool> HasScheduleConflictAsync(
        Guid participantId,
        DateTime startDateTime,
        DateTime endDateTime);

    /// <summary>
    /// Counts the total number of active enrollments for a given workshop.
    /// </summary>
    Task<int> CountActiveByWorkshopIdAsync(
        Guid workshopId);

    /// <summary>
    /// Adds a new enrollment record to the database.
    /// </summary>
    Task AddAsync(Enrollment enrollment);

    /// <summary>
    /// Updates the state of an existing enrollment (e.g., status changes or cancellations).
    /// </summary>
    Task UpdateAsync(Enrollment enrollment);
}
