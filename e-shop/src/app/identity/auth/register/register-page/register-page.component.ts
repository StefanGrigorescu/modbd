import { Component, OnDestroy, OnInit } from '@angular/core';
import { take, takeUntil } from 'rxjs';
import { AuthService } from '../../auth.service';
import { ActivatedRoute, Params, Router, RouterLink } from '@angular/router';
import { AppResponse } from 'common/abstractions/app-response.model';
import { RegisterFormGroup } from '../register.form-group';
import { subscribeToMatchPasswordWithConfirmPassword } from '@forms/passwords/password-with-confirmation.form-group';
import { RegisterForm } from '../register-form.model';
import { SubscriberComponent } from 'src/ui-common/subscriber.component';
import { MatTooltip } from '@angular/material/tooltip';
import { MatButton } from '@angular/material/button';
import { NgIf } from '@angular/common';
import { PasswordComponent } from '../../../../forms/passwords/password/password.component';
import { MatDatepickerInput, MatDatepickerToggle, MatDatepicker } from '@angular/material/datepicker';
import { MatInput } from '@angular/material/input';
import { MatFormField, MatSuffix } from '@angular/material/form-field';
import { TextFieldComponent } from '../../../../forms/text/text-field/text-field.component';
import { EmailComponent } from '../../../../forms/email/email.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrandLogoComponent } from '../../../../../ui-common/brand-logo/brand-logo.component';
import { MatCard } from '@angular/material/card';

@Component({
    selector: 'app-register-page',
    templateUrl: './register-page.component.html',
    styleUrls: ['./register-page.component.scss'],
    standalone: true,
    imports: [MatCard, BrandLogoComponent, FormsModule, ReactiveFormsModule, EmailComponent, TextFieldComponent, MatFormField, MatInput, MatDatepickerInput, MatDatepickerToggle, MatSuffix, MatDatepicker, PasswordComponent, NgIf, MatButton, MatTooltip, RouterLink]
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

    constructor(
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
