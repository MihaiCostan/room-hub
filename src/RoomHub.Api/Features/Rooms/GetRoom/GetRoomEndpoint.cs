using RoomHub.Api.Common.CQRS;
using RoomHub.Api.Common.Http;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Features.Rooms.CreateRoom;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public static class GetRoomEndpoint
{
    public static void Map(RouteGroupBuilder group) => group.MapGet("/{id:guid}", Handle)
        .WithName("GetRoom");

    private static async Task<IResult> Handle(
        Guid id,
        IQueryHandler<GetRoomQuery, Room> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetRoomQuery(id), cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToProblem();
        }

        var room = result.Value;
        return Results.Ok(new RoomResponse(room.Id, room.Name, room.Capacity));
    }
}
