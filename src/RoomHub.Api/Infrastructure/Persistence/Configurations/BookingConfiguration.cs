using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Infrastructure.Persistence.Configurations;

public sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    // SQLite stores DateTime without kind; force UTC when reading back so JSON emits the trailing "Z".
    private static readonly ValueConverter<DateTime, DateTime> UtcConverter = new(
        toDb => toDb,
        fromDb => DateTime.SpecifyKind(fromDb, DateTimeKind.Utc));

    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.RoomId)
            .IsRequired();

        builder.Property(b => b.BookedBy)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Attendees)
            .IsRequired();

        builder.Property(b => b.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.ComplexProperty(b => b.Slot, slot =>
        {
            slot.Property(s => s.StartUtc)
                .IsRequired()
                .HasColumnName("StartUtc")
                .HasConversion(UtcConverter);

            slot.Property(s => s.EndUtc)
                .IsRequired()
                .HasColumnName("EndUtc")
                .HasConversion(UtcConverter);
        });
    }
}
