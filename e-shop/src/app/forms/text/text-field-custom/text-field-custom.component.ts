import { Component, Input, OnInit } from '@angular/core';
import { ValidatorFn, Validators } from '@angular/forms';
import { TextComponent } from '../text-component.model';
import { AppButton } from 'src/ui-common/button.model';

@Component({
    selector: 'app-text-field-custom',
    templateUrl: './text-field-custom.component.html',
    styleUrls: ['./text-field-custom.component.scss']
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
