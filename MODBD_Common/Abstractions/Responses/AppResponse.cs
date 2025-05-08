using System.Diagnostics.CodeAnalysis;
using MODBD_Common.Abstractions.DomainExceptions;
using MODBD_Common.Collections;

namespace MODBD_Common.Abstractions.Responses;

public sealed record AppResponse<T>
{
    private T? Data { get; init; }
    public required IReadOnlyList<ErrorMessage> ErrorMessages { get; init; }
    public required FailureReason? FailureReason { get; init; }
    public required MetadataCollection ErrorMetadata { get; init; }

    [MemberNotNullWhen(true, nameof(Data))]
    [MemberNotNullWhen(false, nameof(FailureReason))]
    public required bool IsSuccess { get; init; }

    [MemberNotNullWhen(true, nameof(FailureReason))]
    [MemberNotNullWhen(false, nameof(Data))]
    public bool IsFailure => !IsSuccess;

    public TResponse Match<TResponse>(
        Func<T, TResponse> onSuccess,
        Func<TResponse> onFailure
    ) =>
        IsSuccess ?
            onSuccess(Data!) :
            onFailure();

    public AppResponse<TResponse> Map<TResponse>(
        Func<T, TResponse> mapWhenIsSuccess
    ) => IsSuccess ?
        AppResponse<TResponse>.Succeeded(mapWhenIsSuccess(Data)) :
        AppResponse<TResponse>.Failed(ErrorMessages, ErrorMetadata, FailureReason);

    public AppResponse<TResponse> Bind<TResponse>(
        Func<T, AppResponse<TResponse>> bindWhenIsSuccess
    ) => IsSuccess ?
        bindWhenIsSuccess(Data) :
        AppResponse<TResponse>.Failed(ErrorMessages, ErrorMetadata, FailureReason);
    
    public AppResponse<TResponse> MapToFailed<TResponse>(
        ErrorMessage? errorMessageWhenIsFailure = null,
        MetadataCollection? errorMetadataWhenIsFailure = null,
        FailureReason? failureReasonWhenIsFailure = null
    ) {
        if (IsSuccess)
        {
            throw new ValueObjectException("Current app response must be failed in order to pass it to this method.");
        }

        return AppResponse<TResponse>.Failed(
            errorMessageWhenIsFailure is null ? ErrorMessages : [errorMessageWhenIsFailure],
            errorMetadataWhenIsFailure ?? ErrorMetadata,
            failureReasonWhenIsFailure ?? FailureReason);
    }

    public bool TryGetData([MaybeNullWhen(false)] out T responseData)
    {
        if (IsSuccess)
        {
            responseData = Data;
            return true;
        }

        responseData = default;
        return false;
    }

    public static AppResponse<T> Failed<TSource>(AppResponse<TSource> response) =>
        response.IsFailure ?
            new()
            {
                IsSuccess = false,
                Data = default,
                ErrorMessages = response.ErrorMessages,
                FailureReason = response.FailureReason,
                ErrorMetadata = response.ErrorMetadata,
            } :
            throw ResponseStatusException.ExpectedFailed;

    public static AppResponse<T> Failed(AppResponse response) =>
        response.IsFailure ?
            new()
            {
                IsSuccess = false,
                Data = default,
                ErrorMessages = response.ErrorMessages,
                FailureReason = response.FailureReason,
                ErrorMetadata = response.ErrorMetadata,
            } :
            throw ResponseStatusException.ExpectedFailed;

    public static AppResponse<T> Failed(
        IReadOnlyList<ErrorMessage> errorMessages,
        MetadataCollection errorMetadata,
        FailureReason reason
    ) => new()
    {
        IsSuccess = false,
        ErrorMessages = errorMessages,
        FailureReason = reason,
        ErrorMetadata = errorMetadata,
    };

    public static AppResponse<T> Failed(
        ErrorMessage errorMessage,
        MetadataCollection errorMetadata,
        FailureReason reason
    ) => new()
    {
        IsSuccess = false,
        ErrorMessages = [errorMessage],
        FailureReason = reason,
        ErrorMetadata = errorMetadata,
    };

    public static AppResponse<T> Failed(
        IReadOnlyList<ErrorMessage> errorMessages,
        MetadataCollection errorMetadata
    ) => new()
    {
        IsSuccess = false,
        ErrorMessages = errorMessages,
        FailureReason = FailureReason.BadRequest,
        ErrorMetadata = errorMetadata,
    };

    public static AppResponse<T> Failed(
        ErrorMessage errorMessage,
        MetadataCollection errorMetadata
    ) => new()
    {
        IsSuccess = false,
        ErrorMessages = [errorMessage],
        FailureReason = FailureReason.BadRequest,
        ErrorMetadata = errorMetadata,
    };

    /// <summary>
    /// Called for succeeded actions.
    /// Sets the <see cref="IsSuccess"/> property to <see langword="false"/>,
    /// the <see cref="Data"/> property to <see langword="null"/>
    /// and the <see cref="ErrorMessages"/> property to the <paramref name="errorMessages"/>.
    /// </summary>
    public static AppResponse<T> Failed(
        IReadOnlyList<ErrorMessage> errorMessages,
        FailureReason reason
    ) => new()
    {
        IsSuccess = false,
        Data = default,
        ErrorMessages = errorMessages,
        FailureReason = reason,
        ErrorMetadata = MetadataCollection.Empty,
    };

    /// <summary>
    /// Called for succeeded actions.
    /// Sets the <see cref="IsSuccess"/> property to <see langword="false"/>,
    /// the <see cref="Data"/> property to <see langword="null"/>
    /// and the <see cref="ErrorMessages"/> property to the <paramref name="errorMessages"/>.
    /// </summary>
    public static AppResponse<T> Failed(
        IReadOnlyList<ErrorMessage> errorMessages
    ) => new()
    {
        IsSuccess = false,
        Data = default,
        ErrorMessages = errorMessages,
        FailureReason = FailureReason.BadRequest,
        ErrorMetadata = MetadataCollection.Empty,
    };

    /// <summary>
    /// Called for succeeded actions.
    /// Sets the <see cref="IsSuccess"/> property to <see langword="false"/>,
    /// the <see cref="Data"/> property to <see langword="null"/>
    /// and the <see cref="ErrorMessages"/> property to a new collection only containing the <paramref name="errorMessage"/> parameter.
    /// </summary>
    public static AppResponse<T> Failed(
        ErrorMessage errorMessage,
        FailureReason reason
    ) => new()
    {
        IsSuccess = false,
        Data = default,
        ErrorMessages = CollectionsFactory.IReadOnlyList(errorMessage),
        FailureReason = reason,
        ErrorMetadata = MetadataCollection.Empty,
    };

    /// <summary>
    /// Called for succeeded actions.
    /// Sets the <see cref="IsSuccess"/> property to <see langword="false"/>,
    /// the <see cref="Data"/> property to <see langword="null"/>
    /// and the <see cref="ErrorMessages"/> property to a new collection only containing the <paramref name="errorMessage"/> parameter.
    /// </summary>
    public static AppResponse<T> Failed(
        ErrorMessage errorMessage
    ) => new()
    {
        IsSuccess = false,
        Data = default,
        ErrorMessages = CollectionsFactory.IReadOnlyList(errorMessage),
        FailureReason = FailureReason.BadRequest,
        ErrorMetadata = MetadataCollection.Empty,
    };

    /// <summary>
    /// Called for succeeded actions.
    /// Sets the <see cref="IsSuccess"/> property to <see langword="true"/>,
    /// the <see cref="Data"/> property to the <paramref name="data"/> parameter
    /// and the <see cref="ErrorMessages"/> property to empty.
    /// </summary>
    public static AppResponse<T> Succeeded(T data)
        => new()
        {
            IsSuccess = true,
            Data = data,
            ErrorMessages = ErrorMessage.EmptyIReadOnlyList,
            FailureReason = null,
            ErrorMetadata = MetadataCollection.Empty,
        };

    public override string ToString() =>
        IsSuccess ?
            $"{nameof(AppResponse)}<{typeof(T).Name}> Succeeded: {Data}" :
            $"{GetType().Name} Failed: {string.Join('|', ErrorMessages)}";

    private AppResponse() {  }
}


public static class AppResponseAsync
{
    public static async Task<AppResponse<TResponse>> MapAsync<T, TResponse>(
        this Task<AppResponse<T>> response,
        Func<T, TResponse> mapWhenIsSuccess
    ) => (await response).Map(mapWhenIsSuccess);

    public static async Task<AppResponse<TResponse>> MapAsync<T, TResponse>(
        this AppResponse<T> response,
        Func<T, Task<TResponse>> mapWhenIsSuccessAsync
    ) => response.TryGetData(out T? responseData) ?
        AppResponse<TResponse>.Succeeded(await mapWhenIsSuccessAsync(responseData)) :
        AppResponse<TResponse>.Failed(response);

    public static async Task<AppResponse<TResponse>> MapAsync<T, TResponse>(
        this Task<AppResponse<T>> response,
        Func<T, Task<TResponse>> mapWhenIsSuccessAsync
    ) => await (await response).MapAsync(mapWhenIsSuccessAsync);


    public static async Task<AppResponse<TResponse>> BindAsync<T, TResponse>(
        this Task<AppResponse<T>> response,
        Func<T, AppResponse<TResponse>> bindWhenIsSuccess
    ) => (await response).Bind(bindWhenIsSuccess);

    public static async Task<AppResponse<TResponse>> BindAsync<T, TResponse>(
        this AppResponse<T> response,
        Func<T, Task<AppResponse<TResponse>>> bindWhenIsSuccessAsync
    ) => response.TryGetData(out T? responseData) ?
        await bindWhenIsSuccessAsync(responseData) :
        AppResponse<TResponse>.Failed(response);

    public static async Task<AppResponse<TResponse>> BindAsync<T, TResponse>(
        this Task<AppResponse<T>> response,
        Func<T, Task<AppResponse<TResponse>>> bindWhenIsSuccessAsync
    ) => await (await response).BindAsync(bindWhenIsSuccessAsync);


    public static async Task<AppResponse<TResponse>> MapToFailedAsync<T, TResponse>(
        this Task<AppResponse<T>> response,
       ErrorMessage? errorMessageWhenIsFailure = null,
       MetadataCollection? errorMetadataWhenIsFailure = null,
       FailureReason? failureReasonWhenIsFailure = null
   ) => (await response).MapToFailed<TResponse>(errorMessageWhenIsFailure, errorMetadataWhenIsFailure, failureReasonWhenIsFailure);


    public static async Task<TResponse> MatchAsync<T, TResponse>(
        this Task<AppResponse<T>> response,
        Func<T, TResponse> onSuccess,
        Func<TResponse> onFailure
    ) => (await response).Match(onSuccess, onFailure);

    public static async Task<TResponse> MatchAsync<T, TResponse>(
        this AppResponse<T> response,
        Func<T, Task<TResponse>> onSuccessAsync,
        Func<TResponse> onFailure
    ) => response.TryGetData(out T? responseData) ?
        await onSuccessAsync(responseData) :
        onFailure();

    public static async Task<TResponse> MatchAsync<T, TResponse>(
        this AppResponse<T> response,
        Func<T, TResponse> onSuccess,
        Func<Task<TResponse>> onFailureAsync
    ) => response.TryGetData(out T? responseData) ?
        onSuccess(responseData) :
        await onFailureAsync();

    public static async Task<TResponse> MatchAsync<T, TResponse>(
        this AppResponse<T> response,
        Func<T, Task<TResponse>> onSuccessAsync,
        Func<Task<TResponse>> onFailureAsync
    ) => response.TryGetData(out T? responseData) ?
        await onSuccessAsync(responseData) :
        await onFailureAsync();

    public static async Task<TResponse> MatchAsync<T, TResponse>(
        this Task<AppResponse<T>> response,
        Func<T, Task<TResponse>> onSuccessAsync,
        Func<TResponse> onFailure
    ) => (await response).TryGetData(out T? responseData) ?
        await onSuccessAsync(responseData) :
        onFailure();

    public static async Task<TResponse> MatchAsync<T, TResponse>(
        this Task<AppResponse<T>> response,
        Func<T, TResponse> onSuccess,
        Func<Task<TResponse>> onFailureAsync
    ) => (await response).TryGetData(out T? responseData) ?
        onSuccess(responseData) :
        await onFailureAsync();

    public static async Task<TResponse> MatchAsync<T, TResponse>(
        this Task<AppResponse<T>> response,
        Func<T, Task<TResponse>> onSuccessAsync,
        Func<Task<TResponse>> onFailureAsync
    ) => (await response).TryGetData(out T? responseData) ?
        await onSuccessAsync(responseData) :
        await onFailureAsync();
}


public sealed record AppResponse
{
    public required IReadOnlyList<ErrorMessage> ErrorMessages { get; init; }
    public required FailureReason? FailureReason { get; init; }
    public required MetadataCollection ErrorMetadata { get; init; }

    [MemberNotNullWhen(false, nameof(FailureReason))]
    public required bool IsSuccess { get; init; }

    [MemberNotNullWhen(true, nameof(FailureReason))]
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Represents succeeded actions.
    /// Sets the <see cref="IsSuccess"/> property to <see langword="true"/>
    /// and the <see cref="ErrorMessages"/> property to empty.
    /// </summary>
    public static readonly AppResponse Succeeded = new()
    {
        IsSuccess = true,
        ErrorMessages = ErrorMessage.EmptyIReadOnlyList,
        FailureReason = null,
        ErrorMetadata = MetadataCollection.Empty,
    };

    public static AppResponse From<T>(AppResponse<T> response) =>
        response.IsFailure ?
            new()
            {
                IsSuccess = false,
                ErrorMessages = response.ErrorMessages,
                FailureReason = response.FailureReason,
                ErrorMetadata = response.ErrorMetadata,
            } :
            Succeeded;

    public static AppResponse Failed<T>(AppResponse<T> response) =>
        response.IsFailure ?
            new()
            {
                IsSuccess = false,
                ErrorMessages = response.ErrorMessages,
                FailureReason = response.FailureReason,
                ErrorMetadata = response.ErrorMetadata,
            } :
            throw ResponseStatusException.ExpectedFailed;

    public static AppResponse Failed(
        IReadOnlyList<ErrorMessage> errorMessages,
        MetadataCollection errorMetadata,
        FailureReason reason
    ) => new()
    {
        IsSuccess = false,
        ErrorMessages = errorMessages,
        FailureReason = reason,
        ErrorMetadata = errorMetadata,
    };

    public static AppResponse Failed(
        ErrorMessage errorMessage,
        MetadataCollection errorMetadata,
        FailureReason reason
    ) => new()
    {
        IsSuccess = false,
        ErrorMessages = [errorMessage],
        FailureReason = reason,
        ErrorMetadata = errorMetadata,
    };

    public static AppResponse Failed(
        IReadOnlyList<ErrorMessage> errorMessages,
        MetadataCollection errorMetadata
    ) => new()
    {
        IsSuccess = false,
        ErrorMessages = errorMessages,
        FailureReason = FailureReason.BadRequest,
        ErrorMetadata = errorMetadata,
    };

    public static AppResponse Failed(
        ErrorMessage errorMessage,
        MetadataCollection errorMetadata
    ) => new()
    {
        IsSuccess = false,
        ErrorMessages = [errorMessage],
        FailureReason = FailureReason.BadRequest,
        ErrorMetadata = errorMetadata,
    };

    /// <summary>
    /// Called for succeeded actions.
    /// Sets the <see cref="IsSuccess"/> property to <see langword="false"/>
    /// and the <see cref="ErrorMessages"/> property to the <paramref name="errorMessages"/>.
    /// </summary>
    public static AppResponse Failed(
        IReadOnlyList<ErrorMessage> errorMessages,
        FailureReason reason
    ) => new()
    {
        IsSuccess = false,
        ErrorMessages = errorMessages,
        FailureReason = reason,
        ErrorMetadata = MetadataCollection.Empty,
    };

    /// <summary>
    /// Called for succeeded actions.
    /// Sets the <see cref="IsSuccess"/> property to <see langword="false"/>
    /// and the <see cref="ErrorMessages"/> property to the <paramref name="errorMessages"/>.
    /// </summary>
    public static AppResponse Failed(
        IReadOnlyList<ErrorMessage> errorMessages
    ) => new()
    {
        IsSuccess = false,
        ErrorMessages = errorMessages,
        FailureReason = FailureReason.BadRequest,
        ErrorMetadata = MetadataCollection.Empty,
    };

    /// <summary>
    /// Called for succeeded actions.
    /// Sets the <see cref="IsSuccess"/> property to <see langword="false"/>
    /// and the <see cref="ErrorMessages"/> property to a new collection only containing the <paramref name="errorMessage"/> parameter.
    /// </summary>
    public static AppResponse Failed(
        ErrorMessage errorMessage,
        FailureReason reason
    ) => new()
    {
        IsSuccess = false,
        ErrorMessages = CollectionsFactory.IReadOnlyList(errorMessage),
        FailureReason = reason,
        ErrorMetadata = MetadataCollection.Empty,
    };

    /// <summary>
    /// Called for succeeded actions.
    /// Sets the <see cref="IsSuccess"/> property to <see langword="false"/>
    /// and the <see cref="ErrorMessages"/> property to a new collection only containing the <paramref name="errorMessage"/> parameter.
    /// </summary>
    public static AppResponse Failed(
        ErrorMessage errorMessage
    ) => new()
    {
        IsSuccess = false,
        ErrorMessages = CollectionsFactory.IReadOnlyList(errorMessage),
        FailureReason = FailureReason.BadRequest,
        ErrorMetadata = MetadataCollection.Empty,
    };

    /// <summary>
    /// This method checks if there are not any errors.<br></br>
    /// If so, it returns the <see cref="Succeeded"/> instance. <br></br>
    /// Otherwise, it calls the <see cref="Failed(IReadOnlyList{ErrorMessage}, Responses.FailureReason)"/>. 
    /// </summary>
    public static AppResponse SucceededIfNoErrorsOrFailedOtherwise(params AppResponse[] appResponses)
        => SucceededIfNoErrorsOrFailedOtherwise(
            appResponses.SelectMany(appResponse => appResponse.ErrorMessages).ToArray()
        );

    /// <summary>
    /// This method checks if there are not any errors.<br></br>
    /// If so, it returns the <see cref="Succeeded"/> instance. <br></br>
    /// Otherwise, it calls the <see cref="Failed(IReadOnlyList{ErrorMessage}, Responses.FailureReason)"/>. 
    /// </summary>
    public static AppResponse SucceededIfNoErrorsOrFailedOtherwise(IReadOnlyList<AppResponse> appResponses)
        => SucceededIfNoErrorsOrFailedOtherwise(
            appResponses.SelectMany(appResponse => appResponse.ErrorMessages).ToArray()
        );

    /// <summary>
    /// This method checks if there are not any errors int the <paramref name="errorMessages"/> collection. <br></br>
    /// If so, it returns the <see cref="Succeeded"/> instance. <br></br>
    /// Otherwise, it calls the <see cref="Failed(IReadOnlyList{ErrorMessage}, Responses.FailureReason)"/>. 
    /// </summary>
    public static AppResponse SucceededIfNoErrorsOrFailedOtherwise(IReadOnlyList<ErrorMessage> errorMessages)
        => errorMessages.Count > 0 ?
            Failed(errorMessages) :
            Succeeded;

    public override string ToString() =>
        IsSuccess ?
            $"{nameof(AppResponse)} Succeeded" :
            $"{nameof(AppResponse)} Failed: {string.Join('|', ErrorMessages)}";

    private AppResponse() {  }
}
