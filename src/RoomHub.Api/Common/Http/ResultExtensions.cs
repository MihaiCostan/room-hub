using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Common.Http;

public static class ResultExtensions
{
    // Maps an Error to the right ProblemDetails HTTP status (400/404/409).
    public static IResult ToProblem(this Error error)
    {
        var status = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest
        };

        return Results.Problem(
            detail: error.Message,
            statusCode: status,
            title: error.Code);
    }
}
