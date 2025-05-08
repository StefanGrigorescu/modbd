// Succeeded response
export type ApiResponse<TResponse> = {
    readonly data: TResponse;
};

export function getApiResponseData<TResponse>(): (response: ApiResponse<TResponse>) => TResponse {
    return (response: ApiResponse<TResponse>) => response.data;
}


// Failed response
export type ApiResponseError = {
    readonly error: ApiResponseFailed;
};

export type ApiResponseFailed = {
    readonly errorMessages: readonly string[];
    readonly additionalData: Readonly<object>;
};
