using RoomHub.Api.Common.CQRS;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed record CreateBookingCommand(
    Guid RoomId,
    string? BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees) : ICommand<Booking>;

public sealed class CreateBookingHandler(
    IRoomRepository rooms,
    IBookingRepository bookings,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ICommandHandler<CreateBookingCommand, Booking>
{
    public async Task<Result<Booking>> HandleAsync(CreateBookingCommand command, CancellationToken cancellationToken = default)
    {
        // 404 before any validation: the parent room must exist first.
        var room = await rooms.GetByIdAsync(command.RoomId, cancellationToken);
        if (room is null)
        {
            return Result<Booking>.Failure(RoomErrors.NotFound(command.RoomId));
        }

        // R3 (+ normalization to UTC).
        var slotResult = TimeSlot.Create(command.StartUtc, command.EndUtc);
        if (slotResult.IsFailure)
        {
            return Result<Booking>.Failure(slotResult.Error);
        }

        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

        // R2, R4, R5, R6, R10.
        var bookingResult = Booking.Create(room, command.BookedBy, slotResult.Value, command.Attendees, nowUtc);
        if (bookingResult.IsFailure)
        {
            return Result<Booking>.Failure(bookingResult.Error);
        }

        var booking = bookingResult.Value;

        // R9: at most 2 active bookings per participant per UTC day, across all rooms.
        var dayUtc = DateOnly.FromDateTime(booking.Slot.StartUtc);
        var sameDayCount = await bookings.CountActiveForParticipantOnDayAsync(booking.BookedBy, dayUtc, cancellationToken);
        if (sameDayCount >= 2)
        {
            return Result<Booking>.Failure(BookingErrors.DailyLimitReached);
        }

        // R7: no overlap with other ACTIVE bookings of the same room (touching allowed).
        var activeForRoom = await bookings.ListActiveForRoomAsync(command.RoomId, cancellationToken);
        if (activeForRoom.Any(existing => existing.Slot.OverlapsWith(booking.Slot)))
        {
            return Result<Booking>.Failure(BookingErrors.RoomOccupied);
        }

        await bookings.AddAsync(booking, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Booking>.Success(booking);
    }
}
