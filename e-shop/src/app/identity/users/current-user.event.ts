import { Injectable } from "@angular/core";
import { BehaviorSubject, filter, map, Observable, Subject, Subscription, takeUntil } from "rxjs";
import { AuthenticatedUser, GuestUser, User } from "./user.model";

/**
 *      This event emitter should be "private", meaning that it should only be available 
 * to the event class and the emitter class (and not interact with it from any other class).
 *      "Singleton pattern" (constant) works very good in this situation for pub-sub, 
 * having a single instance of Event emitter to emit events and accept subscriptions. 
 */
const currentUserEventEmitter: BehaviorSubject<User> = new BehaviorSubject<User>(GuestUser.instance);


@Injectable({
    providedIn: 'root'
})
export class CurrentUserEvent {
    asObservable(): Observable<User> {
        return currentUserEventEmitter.asObservable();
    }

    /**
     * @returns the current user when it is authenticated (and guest users are not emitted at all).
     */
    filterAuthenticated(): Observable<AuthenticatedUser> {
        return currentUserEventEmitter
            .asObservable()
            .pipe(
                filter((user: User) => user.isAuthenticated),
                map((user: AuthenticatedUser) => user),
            );
    }

    subscribe(
        ngUnsubscribe: Subject<void>,
        onEvent: (currentUser: User) => void
    ): Subscription {
        return currentUserEventEmitter
            .pipe(takeUntil(ngUnsubscribe))
            .subscribe((currentUser: User) => onEvent(currentUser));
    }
}


@Injectable({
    providedIn: 'root'
})
export class CurrentUserEventEmitter {
    emit(currentUser: User): void {
        currentUserEventEmitter.next(currentUser);
    }

    getValue(): User {
        return currentUserEventEmitter.getValue();
    }
}
