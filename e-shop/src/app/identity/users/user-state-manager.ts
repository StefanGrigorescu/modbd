import { Injectable } from "@angular/core";
import { AuthenticatedUser, GuestUser, User } from "./user.model";
import { AuthenticatedUserJson } from "./authenticated-user.json";

@Injectable({
    providedIn: 'root',
})
export class UserStateManager {
    private readonly itemKey: string = "user";

    setUser(user: AuthenticatedUser): void {
        localStorage.setItem(this.itemKey, JSON.stringify(user.toJSON()));
    }

    getAuthenticatedUserOrThrow(): AuthenticatedUser {
        const user: User = this.getUser();
        if (!user.isAuthenticated) {
            throw new Error("You need to login first!");
        }
        return user;
    }

    getUser(): User {
        const userString: string | null = localStorage.getItem(this.itemKey) ?? null;
        if (!userString) {
            return GuestUser.instance;
        }

        try {
            const userJson: AuthenticatedUserJson | null = JSON.parse(userString) ?? null;
            if (!userJson) {
                return GuestUser.instance;
            }

            return AuthenticatedUser.fromJson(userJson);
        } catch (error) {
            console.warn(error);
            // Current user will be treated as not authenticated
            this.removeUser();
            return GuestUser.instance;
        }
    }

    removeUser(): void {
        localStorage.removeItem(this.itemKey);
    }
}
