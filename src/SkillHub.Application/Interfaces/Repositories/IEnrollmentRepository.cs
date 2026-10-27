using SkillHub.Domain.Entities;

namespace SkillHub.Application.Interfaces.Repositories;

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetByIdAsync(Guid id);

    Task<IReadOnlyCollection<Enrollment>> GetByParticipantIdAsync(
        Guid participantId);

    Task<IReadOnlyCollection<Enrollment>> GetByWorkshopIdAsync(
        Guid workshopId);

    Task<bool> HasActiveEnrollmentAsync(
        Guid participantId,
        Guid workshopId);

    Task<bool> HasScheduleConflictAsync(
        Guid participantId,
        DateTime startDateTime,
        DateTime endDateTime);

    Task<int> CountActiveByWorkshopIdAsync(
        Guid workshopId);

    Task AddAsync(Enrollment enrollment);
    
    Task UpdateAsync(Enrollment enrollment);
}