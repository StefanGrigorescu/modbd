import { FormControl } from "@angular/forms";

export function getFirstValidationMessage(control: FormControl): string | null {
    if (control.valid) {
        return null;
    }

    const controlErrors = control.errors;

    if (controlErrors?.["required"]) {
        return 'This field is required!';
    }
    if (controlErrors?.["minlength"]) {
        return 'Minimum length not met!';
    }
    if (controlErrors?.["maxlength"]) {
        return 'Maximum length exceeded!';
    }
    if (controlErrors?.["email"]) {
        return 'Invalid email address!';
    }
    if (controlErrors?.["pattern"]) {
        return 'Invalid pattern!';
    }
    if (controlErrors?.["passwordsMatchError"]) {
        return 'Passwords do not match!';
    }

    return 'Please enter a valid value!';
}
