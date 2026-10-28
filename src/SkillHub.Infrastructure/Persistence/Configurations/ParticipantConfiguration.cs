using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillHub.Domain.Entities;

namespace SkillHub.Infrastructure.Persistence.Configurations;

public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.ToTable("Participants");

        builder.HasKey(participant => participant.Id);

        builder.Property(participant => participant.FullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(participant => participant.Email)
            .IsRequired()
            .HasMaxLength(254);

        builder.HasIndex(participant => participant.Email)
            .IsUnique();
    }
}