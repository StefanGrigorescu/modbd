import { FormControl, Validators } from "@angular/forms";

export type PasswordFormControl = FormControl<string>;

export function newPasswordFormControl(): PasswordFormControl {
    return new FormControl<string>("", Validators.required);
}
