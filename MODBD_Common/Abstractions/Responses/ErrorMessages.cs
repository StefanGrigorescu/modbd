namespace MODBD_Common.Abstractions.Responses;

public static class ErrorMessages
{
    /// <summary>
    /// If <paramref name="response"/> status is not Success, add all its error messages to the <paramref name="errorMessages"/> list.
    /// </summary>
    /// <typeparam name="T">The innter type of the <see cref="AppResponse{T}"/> instance.</typeparam>
    /// <param name="errorMessages">The initial error messages list.</param>
    /// <param name="response">The response object to check.</param>
    /// <returns>The same <paramref name="errorMessages"/> instance, 
    /// after conditionally appending <paramref name="response"/>'s error messages to it.</returns>
    public static List<ErrorMessage> AddRangeErrorMessagesIfFailed<T>(this List<ErrorMessage> errorMessages, AppResponse<T> response)
    {
        if (response.IsFailure)
        {
            errorMessages.AddRange(response.ErrorMessages);
        }

        return errorMessages;
    }

    /// <summary>
    /// If <paramref name="response"/> status is not Success, add all its error messages to the <paramref name="errorMessages"/> list.
    /// </summary>
    /// <typeparam name="T">The innter type of the <see cref="AppResponse"/> instance.</typeparam>
    /// <param name="errorMessages">The initial error messages list.</param>
    /// <param name="response">The response object to check.</param>
    /// <returns>The same <paramref name="errorMessages"/> instance, 
    /// after conditionally appending <paramref name="response"/>'s error messages to it.</returns>
    public static List<ErrorMessage> AddRangeErrorMessagesIfFailed(this List<ErrorMessage> errorMessages, AppResponse response)
    {
        if (response.IsFailure)
        {
            errorMessages.AddRange(response.ErrorMessages);
        }

        return errorMessages;
    }
}
