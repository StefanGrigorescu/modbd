using MODBD_Api.Common.Abstractions.Responses;

namespace MODBD_Api.Common.Abstractions;

public readonly struct AppValidationPipe<TIn>
{
    private readonly TIn _data;
    private readonly List<ErrorMessage> _errorMessages = [];
    private readonly bool _isSuccess = true;

    private AppValidationPipe(TIn data)
    {
        _data = data;
    }

    private AppValidationPipe(TIn data, List<ErrorMessage> errorMessages, bool isSuccess)
    {
        _data = data;
        _errorMessages = errorMessages;
        _isSuccess = isSuccess;
    }

    public static AppValidationPipe<TIn> From(TIn data) => new(data);

    public AppValidationPipe<TIn> Next(Func<TIn, AppResponse> validate)
    {
        AppResponse validationResponse = validate(_data);

        List<ErrorMessage> errorMessages = new(_errorMessages);
        errorMessages.AddRangeErrorMessagesIfFailed(validationResponse);

        bool isSuccess = _isSuccess && validationResponse.IsSuccess;

        return new(_data, errorMessages, isSuccess);
    }

    /// <summary>
    /// Returns an <see cref="AppResponse"/> instance for the validation result. <br></br>
    /// It represents either a succeeded action 
    /// or a failed action that contains the <see cref="FailureReason.Validation"/> failure reason and the validation error messages. 
    /// </summary>
    /// <returns></returns>
    public AppResponse ToAppResponse() =>
        _isSuccess ?
        AppResponse.Succeeded :
        AppResponse.Failed(_errorMessages, FailureReason.Validation);
}
