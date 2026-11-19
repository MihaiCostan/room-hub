using RoomHub.Api.Common.CQRS;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public sealed record GetBookingQuery(Guid Id) : IQuery<Booking>;

public sealed class GetBookingHandler(IBookingRepository bookings) : IQueryHandler<GetBookingQuery, Booking>
{
    public async Task<Result<Booking>> HandleAsync(GetBookingQuery query, CancellationToken cancellationToken = default)
    {
        var booking = await bookings.GetByIdAsync(query.Id, cancellationToken);
        if (booking is null)
        {
            return Result<Booking>.Failure(BookingErrors.NotFound(query.Id));
        }

        return Result<Booking>.Success(booking);
    }
}
