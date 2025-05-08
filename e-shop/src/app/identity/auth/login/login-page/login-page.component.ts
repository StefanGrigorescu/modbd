import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router, RouterLink } from '@angular/router';
import { take, takeUntil } from 'rxjs';
import { LoginFormGroup } from '../login.form-group';
import { SubscriberComponent } from 'src/ui-common/subscriber.component';
import { AuthService } from '@identity/auth/auth.service';
import { MatTooltip } from '@angular/material/tooltip';
import { MatButton } from '@angular/material/button';
import { NgIf } from '@angular/common';
import { PasswordComponent } from '../../../../forms/passwords/password/password.component';
import { EmailComponent } from '../../../../forms/email/email.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrandLogoComponent } from '../../../../../ui-common/brand-logo/brand-logo.component';
import { MatCard } from '@angular/material/card';

@Component({
    selector: 'app-login-page',
    templateUrl: './login-page.component.html',
    styleUrls: ['./login-page.component.scss'],
    standalone: true,
    imports: [MatCard, BrandLogoComponent, FormsModule, ReactiveFormsModule, EmailComponent, PasswordComponent, NgIf, MatButton, MatTooltip, RouterLink]
})
export class LoginPageComponent
    extends SubscriberComponent
    implements OnInit, OnDestroy {
    loginForm: LoginFormGroup = new LoginFormGroup();

    get submitBtnTooltip(): string {
        if (!this.loginForm.valid) {
            return "Please fill in all fields and check the form for errors";
        }
        return "Login";
    }

    onSubmit(): void {
        if (!this.loginForm.valid) {
            return;
        }

        this.authService.login(
            this.loginForm.toLoginWithEmailRequest(),
        ).pipe(
            take(1),
        ).subscribe({
            next: _ => {
                this.router.navigate(['/home']);
                this.loginForm.reset();
            },
            error: (err: Error) => {
                this.loginForm.errorMessage = err.message;
            },
        });
    }

    constructor (
        private readonly authService: AuthService,
        private readonly router: Router,
        private readonly route: ActivatedRoute,
    ) { super(); }

    ngOnInit(): void {
        this.route
            .queryParams
            .pipe(takeUntil(this.ngUnsubscribe))
            .subscribe((params: Params) => {
                const email: string | null = params['email'] ?? null;
                if (email !== null) {
                    this.loginForm.emailControl.setValue(email);
                }
            });
    }
}
