import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Router } from "@angular/router";
import { Observable, catchError, map, take, tap } from "rxjs";
import { AuthenticatedUser, GuestUser, User } from "../users/user.model";
import { UserStateManager } from "../users/user-state-manager";
import { LoginForm } from "./login/login-form.model";
import { JwtLoginResponse } from "./login/jwt-login.response";
import { CurrentUserEventEmitter } from "../users/current-user.event";
import { Timer } from "common/time/timer.model";
import { ApiRoutes } from "shared/api-routes";
import { ApiResponse, getApiResponseData } from "shared/api-response.model";
import { AppResponse, appResponseFailedFrom, appResponseSucceededInstance } from "common/abstractions/app-response.model";
import { RegisterForm } from "./register/register-form.model";

@Injectable({
    providedIn: "root",
})
export class AuthService {
    private readonly tokensRefreshTimer: Timer = new Timer();

    constructor (
        private readonly userStateManager: UserStateManager,
        private readonly currentUserEventEmitter: CurrentUserEventEmitter,
        private readonly httpClient: HttpClient,
        private readonly router: Router,
    ) { }

    autoLogin(): void {
        const loadedUser: User = this.userStateManager.getUser();
        this.currentUserEventEmitter.emit(loadedUser);
    }

    login(loginForm: LoginForm): Observable<JwtLoginResponse> {
        const currentUser: User = this.userStateManager.getUser();
        if (currentUser.isAuthenticated) {
            throw new Error(`You are already authenticated as ${currentUser.firstName}. If you want to login with another account, please log out first`);
        }

        return this.httpClient
            .post<ApiResponse<JwtLoginResponse>>(
                ApiRoutes.identity.jwtLoginWithEmail,
                loginForm,
                // { withCredentials: true }
                // withCredentials: true => the returned cookie will automatically be stored on the browser
            ).pipe(
                map(getApiResponseData<JwtLoginResponse>()),
                tap((response: JwtLoginResponse) => {
                    const updatedUser: AuthenticatedUser = AuthenticatedUser.fromJwtLoginResponse(response);
                    this.currentUserEventEmitter.emit(updatedUser);   // emit the new value for our user
                    this.userStateManager.setUser(updatedUser);
                }),
            );
    }

    logout(): void {
        console.log("Logging out...");
        this.tokensRefreshTimer.clearTimeout();
        this.currentUserEventEmitter.emit(GuestUser.instance);
        this.router.navigate(['auth', 'logout']);
        this.userStateManager.removeUser();
    }

    register(registerForm: RegisterForm): Observable<AppResponse> {
        const currentUser: User = this.userStateManager.getUser();
        if (currentUser.isAuthenticated) {
            throw new Error(`You are already authenticated as ${currentUser.firstName}. If you want to register a new account, please log out first`);
        }

        let response: AppResponse = appResponseSucceededInstance;
        return this.httpClient
            .post<Object>(
                ApiRoutes.identity.register,
                registerForm
            ).pipe(
                catchError(async (err: Error) => {
                    response = appResponseFailedFrom(err.message);
                    if (this.userStateManager.getUser().isAuthenticated) {  // Generally, this should not happen
                        this.logout();
                    }
                }),
                take(1),
                map(() => response),
            );
    }
}
