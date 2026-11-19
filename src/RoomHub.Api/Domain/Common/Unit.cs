namespace RoomHub.Api.Domain.Common;

public sealed record Unit
{
    public static readonly Unit Value = new();

    private Unit()
    {
    }
}
