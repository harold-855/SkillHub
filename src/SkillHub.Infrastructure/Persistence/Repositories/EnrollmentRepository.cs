using Microsoft.EntityFrameworkCore;
using SkillHub.Application.Interfaces.Repositories;
using SkillHub.Domain.Entities;
using SkillHub.Domain.Enums;
using SkillHub.Infrastructure.Persistence;

namespace SkillHub.Infrastructure.Persistence.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly SkillHubDbContext _dbContext;

    public EnrollmentRepository(SkillHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Enrollment?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Enrollments
            .FirstOrDefaultAsync(enrollment => enrollment.Id == id);
    }

    public async Task<IReadOnlyCollection<Enrollment>> GetByParticipantIdAsync(
        Guid participantId)
    {
        return await _dbContext.Enrollments
            .Where(enrollment => enrollment.ParticipantId == participantId)
            .OrderByDescending(enrollment => enrollment.EnrolledAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyCollection<Enrollment>> GetByWorkshopIdAsync(
        Guid workshopId)
    {
        return await _dbContext.Enrollments
            .Where(enrollment => enrollment.WorkshopId == workshopId)
            .OrderBy(enrollment => enrollment.EnrolledAt)
            .ToListAsync();
    }

    public async Task<bool> HasActiveEnrollmentAsync(
        Guid participantId,
        Guid workshopId)
    {
        return await _dbContext.Enrollments
            .AnyAsync(enrollment =>
                enrollment.ParticipantId == participantId &&
                enrollment.WorkshopId == workshopId &&
                enrollment.Status == EnrollmentStatus.Active);
    }

    public async Task<bool> HasScheduleConflictAsync(
        Guid participantId,
        DateTime startDateTime,
        DateTime endDateTime)
    {
        return await _dbContext.Enrollments
            .Where(enrollment =>
                enrollment.ParticipantId == participantId &&
                enrollment.Status == EnrollmentStatus.Active)
            .Join(
                _dbContext.Workshops,
                enrollment => enrollment.WorkshopId,
                workshop => workshop.Id,
                (enrollment, workshop) => workshop)
            .AnyAsync(workshop =>
                startDateTime < workshop.EndDateTime &&
                endDateTime > workshop.StartDateTime);
    }

    public async Task<int> CountActiveByWorkshopIdAsync(Guid workshopId)
    {
        return await _dbContext.Enrollments
            .CountAsync(enrollment =>
                enrollment.WorkshopId == workshopId &&
                enrollment.Status == EnrollmentStatus.Active);
    }

    public async Task AddAsync(Enrollment enrollment)
    {
        await _dbContext.Enrollments.AddAsync(enrollment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Enrollment enrollment)
    {
        _dbContext.Enrollments.Update(enrollment);
        await _dbContext.SaveChangesAsync();
    }
}