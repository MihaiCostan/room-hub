namespace RoomHub.Api.Domain.Bookings;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // Only ACTIVE bookings for the room: cancelled ones free their interval (R8).
    Task<IReadOnlyList<Booking>> ListActiveForRoomAsync(Guid roomId, CancellationToken cancellationToken = default);

    // R9: how many ACTIVE bookings this participant has whose startUtc falls on the given UTC date.
    Task<int> CountActiveForParticipantOnDayAsync(string bookedBy, DateOnly dayUtc, CancellationToken cancellationToken = default);

    Task AddAsync(Booking booking, CancellationToken cancellationToken = default);
}
