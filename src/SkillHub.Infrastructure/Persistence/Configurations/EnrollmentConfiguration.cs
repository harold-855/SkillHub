using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillHub.Domain.Entities;

namespace SkillHub.Infrastructure.Persistence.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");

        builder.HasKey(enrollment => enrollment.Id);

        builder.Property(enrollment => enrollment.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(enrollment => enrollment.EnrolledAt)
            .IsRequired();

        builder.Property(enrollment => enrollment.CancelledByUserId)
            .IsRequired(false);

        builder.Property(enrollment => enrollment.CancelledAt)
            .IsRequired(false);

        // One participant can have many enrollments.
        builder.HasOne<Participant>()
            .WithMany(participant => participant.Enrollments)
            .HasForeignKey(enrollment => enrollment.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevents duplicate participant-workshop pairs at the index level.
        builder.HasIndex(enrollment => new
            {
                enrollment.ParticipantId,
                enrollment.WorkshopId
            })
            .HasDatabaseName("IX_Enrollments_ParticipantId_WorkshopId");
    }
}