import { Component, OnDestroy, OnInit } from '@angular/core';
import { take, takeUntil } from 'rxjs';
import { AuthService } from '../../auth.service';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { AppResponse } from 'common/abstractions/app-response.model';
import { RegisterFormGroup } from '../register.form-group';
import { subscribeToMatchPasswordWithConfirmPassword } from '@forms/passwords/password-with-confirmation.form-group';
import { RegisterForm } from '../register-form.model';
import { SubscriberComponent } from 'src/ui-common/subscriber.component';

@Component({
    selector: 'app-register-page',
    templateUrl: './register-page.component.html',
    styleUrls: ['./register-page.component.scss']
})
export class RegisterPageComponent
    extends SubscriberComponent
    implements OnInit, OnDestroy {
    registerForm: RegisterFormGroup | null = null;

    get submitBtnTooltip(): string {
        if (!this.registerForm.valid) {
            return "Please fill in all fields and check the form for errors";
        }
        return "Register";
    }

    onSubmit(): void {
        if (!this.registerForm.valid) {
            return;
        }

        const request: RegisterForm = this.registerForm.toRequest();
        this.authService
            .register(request)
            .pipe(
                take(1),
            ).subscribe({
                next: (response: AppResponse) => {
                    if (!response.isFailure) {
                        this.router.navigate(['auth/login'], { queryParams: { email: request.email, } });
                        return;
                    }
                    this.onRequestFailed(response.errorMessages.join(' '));
                },
                error: (err: Error) => this.onRequestFailed(err.message),
            });
    }

    private onRequestFailed(errorMessage: string): void {
        this.registerForm.errorMessage = errorMessage;
    }

    constructor (
        private readonly authService: AuthService,
        private readonly router: Router,
        private readonly route: ActivatedRoute,
    ) { super(); }

    ngOnInit(): void {
        this.registerForm = new RegisterFormGroup();
        subscribeToMatchPasswordWithConfirmPassword(this.registerForm, this.ngUnsubscribe);

        this.route
            .queryParams
            .pipe(takeUntil(this.ngUnsubscribe))
            .subscribe((params: Params) => {
                const email: string | null = params['email'] ?? null;
                if (email !== null) {
                    this.registerForm.emailControl.setValue(email);
                }

                const errorMessage: string | null = params['errorMessage'] ?? null;
                if (errorMessage !== null) {
                    this.registerForm.errorMessage = errorMessage;
                }
            });
    }
}
