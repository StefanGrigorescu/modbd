export class ApiRoutes {
    static readonly root: string = "https://localhost:44370/api";

    private static readonly _identityRoute: string = ApiRoutes.root + "/identity";
    static readonly identity = {
        register: ApiRoutes._identityRoute + "/register",
        jwtLoginWithEmail: ApiRoutes._identityRoute + "/jwt/login-with-email",
    };
}
