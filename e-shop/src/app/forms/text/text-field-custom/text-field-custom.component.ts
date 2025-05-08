import { Component, Input, OnInit } from '@angular/core';
import { ValidatorFn, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TextComponent } from '../text-component.model';
import { AppButton } from 'src/ui-common/button.model';
import { MatInput } from '@angular/material/input';
import { NgIf } from '@angular/common';
import { MatFormField, MatLabel, MatError, MatSuffix } from '@angular/material/form-field';

@Component({
    selector: 'app-text-field-custom',
    templateUrl: './text-field-custom.component.html',
    styleUrls: ['./text-field-custom.component.scss'],
    standalone: true,
    imports: [MatFormField, NgIf, MatLabel, MatInput, FormsModule, ReactiveFormsModule, MatError, MatSuffix]
})
export class TextFieldCustomComponent
    extends TextComponent
    implements OnInit {
    @Input() readonly fieldTitle: string | null = null;
    @Input() private readonly _customMaxLength: number = 150;
    @Input() readonly inputName: string = "text-field";
    @Input() readonly autocomplete: string | null = null;
    @Input() readonly placeholder: string | null = null;
    @Input() readonly appearance: 'fill' | 'outline' = 'fill';
    @Input() readonly suffixButton: AppButton | null = null;

    getExtraValidators(): ValidatorFn[] {
        return [
            Validators.maxLength(this._customMaxLength),
        ];
    }
}
