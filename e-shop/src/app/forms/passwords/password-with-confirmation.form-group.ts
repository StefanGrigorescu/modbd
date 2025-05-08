import { AbstractControl, FormGroup, ValidatorFn, Validators } from "@angular/forms";
import { PasswordFormControl } from "./password.form-control";
import { Subject, takeUntil } from "rxjs";
import { StringFormControl } from "@forms/text/string.form-control";
import { ValidationFailure } from "@forms/validation/validation-failure.model";

export type PasswordWithConfirmationFormGroup = FormGroup<{
    readonly password: PasswordFormControl;
    readonly confirmPassword: StringFormControl;
}>;

export function newPasswordWithConfirmationFormGroup(passwordControl: PasswordFormControl): PasswordWithConfirmationFormGroup {
    return new FormGroup({
        password: passwordControl,
        confirmPassword: new StringFormControl([
            Validators.required,
            initPasswordsMatchValidator(passwordControl),
        ]),
    });
}

function initPasswordsMatchValidator(passwordControl: PasswordFormControl): ValidatorFn {
    return (confirmPasswordControl: AbstractControl<any, any>): ValidationFailure | null => {
        if (!passwordControl || !confirmPasswordControl) {
            return { formControlNullError: true };
        }

        return passwordControl.value !== confirmPasswordControl.value ?
            { passwordsMatchError: true } :
            {};
    };
}


export type WithPasswordWithConfirmationFormControls = {
    readonly passwordControl: PasswordFormControl;
    readonly confirmPasswordControl: StringFormControl;
};

export function subscribeToMatchPasswordWithConfirmPassword(
    form: WithPasswordWithConfirmationFormControls,
    ngUnsubscribe: Subject<void>,
): void {
    // subscribe to password value changes
    form
        .passwordControl
        .valueChanges
        .pipe(takeUntil(ngUnsubscribe))
        .subscribe(() => {
            form
                .confirmPasswordControl
                .updateValueAndValidity();
        });
}
