using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillHub.Domain.Entities;

namespace SkillHub.Infrastructure.Persistence.Configurations;

public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
{
    public void Configure(EntityTypeBuilder<Instructor> builder)
    {
        builder.ToTable("Instructors");

        builder.HasKey(instructor => instructor.Id);

        builder.Property(instructor => instructor.FullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(instructor => instructor.Email)
            .IsRequired()
            .HasMaxLength(254);

        builder.Property(instructor => instructor.Specialty)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(instructor => instructor.Email)
            .IsUnique();
    }
}