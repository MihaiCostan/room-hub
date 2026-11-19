using RoomHub.Api.Common.CQRS;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public sealed record GetRoomQuery(Guid Id) : IQuery<Room>;

public sealed class GetRoomHandler(IRoomRepository rooms) : IQueryHandler<GetRoomQuery, Room>
{
    public async Task<Result<Room>> HandleAsync(GetRoomQuery query, CancellationToken cancellationToken = default)
    {
        var room = await rooms.GetByIdAsync(query.Id, cancellationToken);
        if (room is null)
        {
            return Result<Room>.Failure(RoomErrors.NotFound(query.Id));
        }

        return Result<Room>.Success(room);
    }
}
