import { FormGroup, Validators } from "@angular/forms";
import { newPasswordWithConfirmationFormGroup, PasswordWithConfirmationFormGroup, WithPasswordWithConfirmationFormControls } from "../../../forms/passwords/password-with-confirmation.form-group";
import { newPasswordFormControl, PasswordFormControl } from "../../../forms/passwords/password.form-control";
import { StringFormControl } from "@forms/text/string.form-control";
import { RegisterForm } from "./register-form.model";
import { DateFormControl } from "@forms/time/date-form-control.model";

export class RegisterFormGroup extends FormGroup<{
    userData: UserDataFormGroup;
    secret: PasswordWithConfirmationFormGroup;
}> implements WithPasswordWithConfirmationFormControls {
    readonly emailControl: StringFormControl;
    readonly firstNameControl: StringFormControl;
    readonly lastNameControl: StringFormControl;
    readonly phoneNumberControl: StringFormControl;
    readonly dateOfBirthControl: DateFormControl;
    readonly passwordControl: PasswordFormControl;
    readonly confirmPasswordControl: StringFormControl;

    errorMessage: string | null = null;

    constructor () {
        super({
            userData: new FormGroup({
                email: new StringFormControl(Validators.required),
                firstName: new StringFormControl(Validators.required),
                lastName: new StringFormControl(Validators.required),
                phoneNumber: new StringFormControl(Validators.required),
                dateOfBirth: new DateFormControl(Validators.required),
            }),
            secret: newPasswordWithConfirmationFormGroup(newPasswordFormControl()),
        });

        this.emailControl = this.controls.userData.controls.email;
        this.firstNameControl = this.controls.userData.controls.firstName;
        this.lastNameControl = this.controls.userData.controls.lastName;
        this.phoneNumberControl = this.controls.userData.controls.phoneNumber;
        this.dateOfBirthControl = this.controls.userData.controls.dateOfBirth;
        this.passwordControl = this.controls.secret.controls.password;
        this.confirmPasswordControl = this.controls.secret.controls.confirmPassword;
    }

    toRequest(): RegisterForm {
        return {
            email: this.emailControl.valueOrEmpty,
            firstName: this.firstNameControl.valueOrEmpty,
            lastName: this.lastNameControl.valueOrEmpty,
            phoneNumber: this.phoneNumberControl.valueOrEmpty,
            dateOfBirth: this.dateOfBirthControl.value,
            password: this.passwordControl.value,
        };
    }
}


type UserDataFormGroup = FormGroup<{
    readonly email: StringFormControl;
    readonly firstName: StringFormControl;
    readonly lastName: StringFormControl;
    readonly phoneNumber: StringFormControl;
    readonly dateOfBirth: DateFormControl;
}>;
