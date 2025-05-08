import { AsyncValidatorFn, FormControl, FormControlOptions, ValidatorFn } from "@angular/forms";
import { NullableDate } from "common/time/nullable-date.model";

/**
 * Date Form Control (may include time too)
 */
export class DateFormControl extends FormControl<NullableDate> {
    constructor (
        validatorOrOpts?: ValidatorFn | ValidatorFn[] | FormControlOptions | null, asyncValidator?: AsyncValidatorFn | AsyncValidatorFn[] | null,
    ) { super(null, validatorOrOpts); }
}
