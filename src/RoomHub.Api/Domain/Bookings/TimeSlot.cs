using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

public sealed record TimeSlot
{
    public static readonly TimeSpan MaxDuration = TimeSpan.FromHours(4);

    private TimeSlot()
    {
    }

    public DateTime StartUtc { get; private set; }

    public DateTime EndUtc { get; private set; }

    // R3: end strictly after start. R6: duration at most 4 hours (exactly 4h allowed).
    public static Result<TimeSlot> Create(DateTime startUtc, DateTime endUtc)
    {
        var start = startUtc.ToUniversalTime();
        var end = endUtc.ToUniversalTime();

        if (end <= start)
        {
            return Result<TimeSlot>.Failure(BookingErrors.InvalidRange);
        }

        if (end - start > MaxDuration)
        {
            return Result<TimeSlot>.Failure(BookingErrors.DurationTooLong);
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
