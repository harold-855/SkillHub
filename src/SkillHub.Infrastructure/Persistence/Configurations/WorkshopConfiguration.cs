using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillHub.Domain.Entities;

namespace SkillHub.Infrastructure.Persistence.Configurations;

public class WorkshopConfiguration : IEntityTypeConfiguration<Workshop>
{
    public void Configure(EntityTypeBuilder<Workshop> builder)
    {
        builder.ToTable("Workshops");

        builder.HasKey(workshop => workshop.Id);

        builder.Property(workshop => workshop.Code)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasIndex(workshop => workshop.Code)
            .IsUnique();

        builder.Property(workshop => workshop.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(workshop => workshop.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(workshop => workshop.StartDateTime)
            .IsRequired();

        builder.Property(workshop => workshop.EndDateTime)
            .IsRequired();

        builder.Property(workshop => workshop.MaximumCapacity)
            .IsRequired();

        // One instructor can teach many workshops.
        builder.HasOne<Instructor>()
            .WithMany(instructor => instructor.Workshops)
            .HasForeignKey(workshop => workshop.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        // One workshop can have many enrollments.
        builder.HasMany(workshop => workshop.Enrollments)
            .WithOne()
            .HasForeignKey(enrollment => enrollment.WorkshopId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}