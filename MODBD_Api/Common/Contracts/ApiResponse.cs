using Microsoft.AspNetCore.Mvc;
using MODBD_Api.Common.Abstractions.Responses;
using MODBD_Api.Common.Collections;

namespace MODBD_Api.Common.Contracts;

public sealed record ApiSuccessResponse<T>
    where T : class
{
    public required T Data { get; init; }

    public static ApiSuccessResponse<T> From(AppResponse<T> source) => new()
    {
        Data = ResponseStatusException.ThrowIfIsNotSucceeded(source),
    };
    private ApiSuccessResponse() {  }
}


public record ApiFailedResponse
{
    public required IReadOnlyList<string> ErrorMessages { get; init; }
    public required MetadataCollection ErrorMetadata { get; init; }

    public static ApiFailedResponse From<T>(AppResponse<T> source) => new()
    {
        ErrorMessages = source.ErrorMessages
            .ToIReadOnlyList(msg => msg.ToString()),
        ErrorMetadata = source.ErrorMetadata,
    };

    public static ApiFailedResponse From(AppResponse source) => new()
    {
        ErrorMessages = source.ErrorMessages
        .ToIReadOnlyList(msg => msg.ToString()),
        ErrorMetadata = source.ErrorMetadata,
    };

    public static ApiFailedResponse FromErrorMessage(string errorMessage) => new()
    { 
        ErrorMessages = [errorMessage],
        ErrorMetadata = MetadataCollection.Empty,
    };

    private ApiFailedResponse() { }
}


public static class IActionResultFactory
{
    public static IActionResult From<TController, TResponse>(this TController controller, AppResponse<TResponse> source)
        where TController : ControllerBase
        where TResponse : class
    {
        if (source.IsSuccess)
        {
            return controller.Ok(
                ApiSuccessResponse<TResponse>.From(source));
        }

        if(source.FailureReason == FailureReason.Unauthorized)
        {
            return controller.Unauthorized(
                ApiFailedResponse.From(source));
        }

        if(source.FailureReason == FailureReason.LockedOut)
        {
            return controller.StatusCode(
                StatusCodes.Status423Locked,
                ApiFailedResponse.From(source)
            );
        }

        return controller.BadRequest(
            ApiFailedResponse.From(source));
    }

    public static IActionResult From<TController>(this TController controller, AppResponse source)
        where TController : ControllerBase
    {
        if (source.IsSuccess)
        {
            return controller.NoContent();
        }

        if (source.FailureReason == FailureReason.Unauthorized)
        {
            return controller.Unauthorized(
                ApiFailedResponse.From(source));
        }

        if (source.FailureReason == FailureReason.LockedOut)
        {
            return controller.StatusCode(
                StatusCodes.Status423Locked,
                ApiFailedResponse.From(source)
            );
        }

        return controller.BadRequest(
            ApiFailedResponse.From(source));
    }
}
