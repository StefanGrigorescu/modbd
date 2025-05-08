import { AccessJwtResponse } from "../access-jwt.model";
import { Roles } from "../../role.model";

export type JwtLoginResponse = {
    readonly id: string;
    readonly firstName: string;
    readonly lastName: string;
    readonly email: string;
    readonly roles: Roles;
    readonly accessJwt: AccessJwtResponse;
};
