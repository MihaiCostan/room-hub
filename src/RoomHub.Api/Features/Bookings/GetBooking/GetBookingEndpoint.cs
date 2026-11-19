using RoomHub.Api.Common.CQRS;
using RoomHub.Api.Common.Http;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public sealed record BookingResponse(
    Guid Id,
    Guid RoomId,
    string BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees,
    string Status);

public static class GetBookingEndpoint
{
    public static void Map(RouteGroupBuilder group) => group.MapGet("/{id:guid}", Handle)
        .WithName("GetBooking");

    private static async Task<IResult> Handle(
        Guid id,
        IQueryHandler<GetBookingQuery, Booking> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetBookingQuery(id), cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToProblem();
        }

        var b = result.Value;
        return Results.Ok(new BookingResponse(
            b.Id, b.RoomId, b.BookedBy, b.Slot.StartUtc, b.Slot.EndUtc, b.Attendees, b.Status.ToString()));
    }
}
