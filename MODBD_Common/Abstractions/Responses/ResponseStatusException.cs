using MODBD_Common.Abstractions.DomainExceptions;

namespace MODBD_Common.Abstractions.Responses;

public sealed class ResponseStatusException : ValueObjectException
{
    public static T ThrowIfIsNotSucceeded<T>(AppResponse<T> response)
    {
        if (!response.TryGetData(out T? data))
        {
            throw ExpectedSucceeded;
        }
        return data;
    }

    public static void ThrowIfIsNotSucceeded(AppResponse response) =>
        ExpectedSucceeded.ThrowIf(response.IsFailure);

    public static void ThrowIfIsNotFailed<T>(AppResponse<T> response) =>
        ExpectedFailed.ThrowIf(response.IsSuccess);

    public static readonly ResponseStatusException ExpectedFailed = new("Response failed was expected. Actual response is succeeded.");
    public static readonly ResponseStatusException ExpectedSucceeded = new("Response succeeded was expected. Actual response is failed.");

    private ResponseStatusException(string message) : base(message) { }
}
