import { Component, Input, OnInit } from "@angular/core";
import { FormControl, ValidatorFn, Validators } from "@angular/forms";
import { getFirstValidationMessage } from "../validation/get-first-validation-message.function";

@Component({
    template: '',
    styleUrls: []
})
export abstract class TextComponent implements OnInit {
    @Input() readonly control!: FormControl<string>;
    validationMessage: string | null = null;

    ngOnInit(): void {
        if (!this.control) {
            throw new Error('FormControl is required!');
        }

        this.control.setValidators(Validators.compose([
            ...this.control.validator ? [this.control.validator] : [],
            ...this.getExtraValidators(),
        ]));
        this.control.updateValueAndValidity();

        this.control
            .valueChanges
            .subscribe(() => {
                this.updateValidationMessage();
            });
    }

    updateValidationMessage(): void {
        const showError = this.control.touched && !this.control.valid;

        this.validationMessage = showError ?
            getFirstValidationMessage(this.control) :
            null;
    }

    protected abstract getExtraValidators(): ValidatorFn[];
}
