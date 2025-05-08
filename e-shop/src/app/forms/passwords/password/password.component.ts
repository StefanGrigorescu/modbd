import { Component, Input, OnInit } from '@angular/core';
import { ValidatorFn, Validators } from '@angular/forms';
import { getFirstValidationMessage } from '@forms/validation/get-first-validation-message.function';
import { passwordMaxLength } from '@forms/text/text-max-lengths.const';
import { TextComponent } from '@forms/text/text-component.model';

@Component({
    selector: 'app-password',
    templateUrl: './password.component.html',
    styleUrls: ['./password.component.scss']
})
export class PasswordComponent
    extends TextComponent
    implements OnInit {
    @Input() readonly fieldTitle: string = "Password";
    @Input() readonly autocomplete: string | null = null;
    @Input() readonly appearance: 'fill' | 'outline' = 'fill';

    getExtraValidators(): ValidatorFn[] {
        return [
            Validators.pattern(/^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*\W).*$/),
            Validators.minLength(8),
            Validators.maxLength(passwordMaxLength),
        ];
    }

    /**
    * Overrides validation messages for certain validations. 
    * If those validations are not faulted, it calls the function responsible for all validation messages.  
    */
    override updateValidationMessage(): void {
        const showError = this.control.touched && !this.control.valid;

        if (!showError) {
            this.validationMessage = null;
            return;
        }

        const controlErrors = this.control.errors;

        if (controlErrors?.['pattern']) {
            this.validationMessage = 'Password needs to have at least one digit, one lower case letter, one upper case letter and a special character (e.g. !, @, #, $, etc.)!';
            return;
        }
        if (controlErrors?.['minlength']) {
            this.validationMessage = 'Password needs to have at least 8 characters!';
            return;
        }

        this.validationMessage = getFirstValidationMessage(this.control);
    };
}
