import { FormGroup, Validators } from "@angular/forms";
import { StringFormControl } from "@forms/text/string.form-control";
import { LoginForm } from "./login-form.model";

export class LoginFormGroup extends FormGroup<{
    readonly email: StringFormControl,
    readonly password: StringFormControl,
}> {
    readonly emailControl: StringFormControl;
    readonly passwordControl: StringFormControl;

    errorMessage: string | null = null;

    constructor () {
        super({
            email: new StringFormControl(Validators.required),
            password: new StringFormControl(Validators.required),
        });

        this.emailControl = this.controls.email;
        this.passwordControl = this.controls.password;
    }

    toLoginWithEmailRequest(): LoginForm {
        return {
            email: this.emailControl.valueOrEmpty,
            password: this.passwordControl.valueOrEmpty,
        };
    }
}
