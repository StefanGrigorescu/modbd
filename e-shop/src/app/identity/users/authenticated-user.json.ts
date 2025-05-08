
import { AccessJwtJson } from "@identity/auth/access-jwt.model";
import { Roles } from "../role.model";

/**
 * Represents an Authenticated User stringified as json to be saved in local storage for later loading. 
 */
export type AuthenticatedUserJson = {
    readonly id: string;
    readonly firstName: string;
    readonly lastName: string;
    readonly email: string;
    readonly _roles: Roles;
    readonly accessJwt: AccessJwtJson;
};
