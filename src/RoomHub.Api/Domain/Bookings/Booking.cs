using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Domain.Bookings;

public sealed class Booking
{
    // R6: default max duration 4h. R10: rooms with capacity > 10 are limited to 2h.
    public static readonly TimeSpan DefaultMaxDuration = TimeSpan.FromHours(4);
    public static readonly TimeSpan LargeRoomMaxDuration = TimeSpan.FromHours(2);
    public const int LargeRoomCapacityThreshold = 10;

    // R11: a booking can be cancelled only if it starts at least this long from now.
    public static readonly TimeSpan MinCancelLeadTime = TimeSpan.FromMinutes(60);

    private Booking()
    {
    }

    public Guid Id { get; private set; }

    public Guid RoomId { get; private set; }

    public string BookedBy { get; private set; } = null!;

    public TimeSlot Slot { get; private set; } = null!;

    public int Attendees { get; private set; }

    public BookingStatus Status { get; private set; }

    public bool IsActive => Status == BookingStatus.Active;

    // R2: bookedBy required. R4: attendees positive and at most room capacity.
    // R5: cannot start in the past ("now" comes from the caller / TimeProvider).
    public static Result<Booking> Create(
        Room room,
        string? bookedBy,
        TimeSlot slot,
        int attendees,
        DateTime nowUtc)
    {
        if (string.IsNullOrWhiteSpace(bookedBy))
        {
            return Result<Booking>.Failure(BookingErrors.InvalidBookedBy);
        }

        if (attendees <= 0)
        {
            return Result<Booking>.Failure(BookingErrors.InvalidAttendees);
        }

        if (attendees > room.Capacity)
        {
            return Result<Booking>.Failure(BookingErrors.AttendeesExceedCapacity);
        }

        // R6 + R10: max duration depends on room capacity.
        var maxDuration = room.Capacity > LargeRoomCapacityThreshold
            ? LargeRoomMaxDuration
            : DefaultMaxDuration;

        if (slot.Duration > maxDuration)
        {
            return Result<Booking>.Failure(BookingErrors.DurationTooLong);
        }

        if (slot.StartUtc < nowUtc)
        {
            return Result<Booking>.Failure(BookingErrors.StartInThePast);
        }

        return Result<Booking>.Success(new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = room.Id,
            BookedBy = bookedBy.Trim(),
            Slot = slot,
            Attendees = attendees,
            Status = BookingStatus.Active
        });
    }

    // R8: cancelling an already cancelled booking is a conflict.
    // R11: cannot cancel if the booking starts less than 60 minutes from now.
    public Result<Unit> Cancel(DateTime nowUtc)
    {
        if (Status == BookingStatus.Cancelled)
        {
            return Result<Unit>.Failure(BookingErrors.AlreadyCancelled);
        }

        if (Slot.StartUtc - nowUtc < MinCancelLeadTime)
        {
            return Result<Unit>.Failure(BookingErrors.CancelTooLate);
        }

        Status = BookingStatus.Cancelled;
        return Result<Unit>.Success(Unit.Value);
    }
}
