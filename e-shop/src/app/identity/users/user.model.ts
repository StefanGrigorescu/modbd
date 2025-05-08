import { AccessJwt } from "@identity/auth/access-jwt.model";
import { Roles } from "../role.model";
import { AuthenticatedUserJson } from "./authenticated-user.json";
import { JwtLoginResponse } from "@identity/auth/login/jwt-login.response";

export type User = AuthenticatedUser | GuestUser;


export class GuestUser {
    readonly isAuthenticated: false = false;

    static readonly instance: GuestUser = new GuestUser();

    private constructor () { }
}


export class AuthenticatedUser {
    readonly id: string;
    readonly email: string;
    readonly firstName: string;
    readonly lastName: string;
    private readonly _roles: Roles;
    accessJwt: AccessJwt;

    readonly isAuthenticated: true = true;

    private constructor (params: AuthenticatedUserCtorParams) {
        this.id = params.id;
        this.email = params.email;
        this.firstName = params.firstName;
        this.lastName = params.lastName;
        this._roles = params.roles;
        this.accessJwt = params.accessJwt;

        this.validateDataIntegrity();
    }

    static fromJwtLoginResponse(response: JwtLoginResponse): AuthenticatedUser {
        return new AuthenticatedUser({
            id: response.id,
            email: response.email,
            firstName: response.firstName,
            lastName: response.lastName,
            roles: response.roles,
            accessJwt: AccessJwt.fromResponse(response.accessJwt),
        });
    }

    static fromJson(json: AuthenticatedUserJson): AuthenticatedUser {
        return new AuthenticatedUser({
            id: json.id,
            email: json.email,
            firstName: json.firstName,
            lastName: json.lastName,
            roles: json._roles,
            accessJwt: AccessJwt.fromJson(json.accessJwt),
        });
    }

    toJSON(): AuthenticatedUserJson {
        return {
            id: this.id,
            email: this.email,
            firstName: this.firstName,
            lastName: this.lastName,
            _roles: this._roles,
            accessJwt: this.accessJwt.toJson(),
        };
    }

    validateDataIntegrity(): void {
        if (
            !this.id ||
            !this.email ||
            !this.firstName ||
            !this.lastName ||
            !this._roles ||
            !this.accessJwt ||
            !this.accessJwt.expireTime ||
            !this.accessJwt.value
        ) {
            throw new Error("User data was not available!");
        }
    }
}


type AuthenticatedUserCtorParams = {
    readonly id: string;
    readonly email: string;
    readonly firstName: string;
    readonly lastName: string;
    readonly roles: Roles;
    readonly accessJwt: AccessJwt;
};
