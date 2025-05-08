export type AppResponseWithData<TData> = AppResponseSucceededWithData<TData> | AppResponseFailed;

export class AppResponseSucceededWithData<TData> {
    readonly isSuccess: true;
    readonly isFailure: false;
    readonly data: TData;

    static from<TData>(data: TData): AppResponseSucceededWithData<TData> {
        return new AppResponseSucceededWithData<TData>(data);
    }

    private constructor (data: TData) {
        this.isSuccess = true;
        this.isFailure = false;
        this.data = data;
    }
}


export type AppResponse = AppResponseSucceeded | AppResponseFailed;

export type AppResponseSucceeded = {
    readonly isSuccess: true;
    readonly isFailure: false;
};

export type AppResponseFailed = {
    readonly isSuccess: false;
    readonly isFailure: true;
    readonly errorMessages: string[];
};


export const appResponseSucceededInstance: AppResponseSucceeded = {
    isSuccess: true,
    isFailure: false,
};

export function appResponseFailedFrom(errMessages: string | string[]): AppResponseFailed {
    if (typeof errMessages === "string") {
        errMessages = [errMessages];
    }

    return {
        isSuccess: false,
        isFailure: true,
        errorMessages: errMessages,
    };
}

export function mapAppResponseWithData<TIn, TOut>(
    appResponseWithData: AppResponseWithData<TIn>,
    mapWhenIsSuccess: (dataIn: TIn) => TOut): AppResponseWithData<TOut> {
    if (appResponseWithData.isSuccess) {
        return AppResponseSucceededWithData.from(
            mapWhenIsSuccess(appResponseWithData.data)
        );
    }
    if (appResponseWithData.isFailure) {
        return appResponseFailedFrom(
            appResponseWithData.errorMessages
        );
    }
    throw Error("Invalid AppResponseWithData");
}
