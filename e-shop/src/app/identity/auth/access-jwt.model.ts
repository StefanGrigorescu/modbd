import { dateFromLocaleString, dateFromUTCString } from "common/time/date.factory";

export class AccessJwt {
    private constructor (
        readonly value: string,
        readonly expireTime: Date,
    ) { }

    static fromResponse(response: AccessJwtResponse): AccessJwt {
        return new AccessJwt(
            response.value,
            dateFromUTCString(response.expireTime),
        );
    }

    static fromJson(json: AccessJwtJson): AccessJwt {
        return new AccessJwt(
            json.value,
            dateFromLocaleString(json.expireTime),
        );
    }

    toJson(): AccessJwtJson {
        return {
            value: this.value,
            expireTime: this.expireTime.toJSON(),
        };
    }
}


export type AccessJwtResponse = {
    readonly value: string;
    readonly expireTime: string;
};

export type AccessJwtJson = {
    readonly value: string;
    readonly expireTime: string;
};
