using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Infrastructure.Persistence.Repository;

public sealed class BookingRepository(RoomHubDbContext dbContext) : IBookingRepository
{
    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Bookings.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Booking>> ListActiveForRoomAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Bookings
            .Where(b => b.RoomId == roomId && b.Status == BookingStatus.Active)
            .ToListAsync(cancellationToken);
    }

    // R9: count ACTIVE bookings for this participant whose startUtc falls on the given UTC day.
    public Task<int> CountActiveForParticipantOnDayAsync(string bookedBy, DateOnly dayUtc, CancellationToken cancellationToken = default)
    {
        var dayStart = dayUtc.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var dayEnd = dayStart.AddDays(1);

        return dbContext.Bookings
            .Where(b => b.Status == BookingStatus.Active
                        && b.BookedBy == bookedBy
                        && b.Slot.StartUtc >= dayStart
                        && b.Slot.StartUtc < dayEnd)
            .CountAsync(cancellationToken);
    }

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        await dbContext.Bookings.AddAsync(booking, cancellationToken);
    }
}
