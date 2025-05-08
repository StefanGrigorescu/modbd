import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, CanActivateChild, Router, RouterStateSnapshot, UrlTree } from "@angular/router";
import { Observable, map, take } from 'rxjs';
import { User } from "../users/user.model";
import { CurrentUserEvent } from "../users/current-user.event";

@Injectable({
    providedIn: "root",
})
export class AlreadyLoggedInGuard implements CanActivate, CanActivateChild {
    constructor (
        private readonly currentUserEvent: CurrentUserEvent,
        private readonly router: Router,
    ) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot):
        boolean |
        UrlTree |
        Observable<boolean |
            UrlTree> |
        Promise<boolean |
            UrlTree> {
        return this.currentUserEvent
            .asObservable()
            .pipe(
                take(1),
                map((user: User) => {
                    if (user.isAuthenticated) {  // If the user is already authenticated, they shouldn't be able to access this route
                        return this.router.parseUrl("/");  // Redirect to the home page
                    }
                    return true;
                }),
            );
    }

    canActivateChild(
        childRoute: ActivatedRouteSnapshot,
        state: RouterStateSnapshot): boolean | UrlTree | Observable<boolean | UrlTree> | Promise<boolean | UrlTree> {
        return this.canActivate(childRoute, state);
    }
}
