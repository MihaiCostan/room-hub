using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

public sealed record TimeSlot
{
    private TimeSlot()
    {
    }

    public DateTime StartUtc { get; private set; }

    public DateTime EndUtc { get; private set; }

    public TimeSpan Duration => EndUtc - StartUtc;

    // R3: end strictly after start. (Max duration depends on room capacity, enforced in Booking.Create: R6 + R10.)
    public static Result<TimeSlot> Create(DateTime startUtc, DateTime endUtc)
    {
        var start = startUtc.ToUniversalTime();
        var end = endUtc.ToUniversalTime();

        if (end <= start)
        {
            return Result<TimeSlot>.Failure(BookingErrors.InvalidRange);
        }

        return Result<TimeSlot>.Success(new TimeSlot
        {
            StartUtc = start,
            EndUtc = end
        });
    }

    // R7: two intervals overlap only if they strictly cross. Touching (end == start) does NOT overlap.
    public bool OverlapsWith(TimeSlot other) =>
        StartUtc < other.EndUtc && other.StartUtc < EndUtc;
}
