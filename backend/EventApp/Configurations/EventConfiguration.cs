using EventApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventApp.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).IsRequired().HasMaxLength(2000);
        builder.Property(e => e.Date).IsRequired();
        builder.Property(e => e.Location).IsRequired().HasMaxLength(300);
        builder.Property(e => e.Capacity).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
    }
}
