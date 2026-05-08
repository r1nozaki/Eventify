using EventApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventApp.Configurations;

public class ParticipantRegistrationConfiguration : IEntityTypeConfiguration<ParticipantRegistration>
{
    public void Configure(EntityTypeBuilder<ParticipantRegistration> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Status).IsRequired();
        builder.Property(r => r.CreatedAt).IsRequired();

        builder.HasOne(r => r.User)
            .WithMany(u => u.Registrations)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Event)
            .WithMany(e => e.Registrations)
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => new { r.UserId, r.EventId }).IsUnique();
    }
}
