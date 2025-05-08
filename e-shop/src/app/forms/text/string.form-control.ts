import { AsyncValidatorFn, FormControl, FormControlOptions, ValidatorFn } from "@angular/forms";

export class StringFormControl extends FormControl<string | null> {
    get valueOrEmpty(): string { return this.value ?? ""; }

    constructor (
        validatorOrOpts?: ValidatorFn | ValidatorFn[] | FormControlOptions | null, asyncValidator?: AsyncValidatorFn | AsyncValidatorFn[] | null,
    ) { super("", validatorOrOpts); }
}
