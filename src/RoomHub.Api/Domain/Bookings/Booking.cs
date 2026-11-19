using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Domain.Bookings;

public sealed class Booking
{
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
    public Result<Unit> Cancel()
    {
        if (Status == BookingStatus.Cancelled)
        {
            return Result<Unit>.Failure(BookingErrors.AlreadyCancelled);
        }

        Status = BookingStatus.Cancelled;
        return Result<Unit>.Success(Unit.Value);
    }
}
