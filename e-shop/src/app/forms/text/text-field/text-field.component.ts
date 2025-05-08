import { Component, Input, OnInit } from '@angular/core';
import { ValidatorFn, Validators } from '@angular/forms';
import { TextFieldType, defaultTextFieldType, getTextFieldMaxLength } from '../text-field.type.enum';
import { TextComponent } from '../text-component.model';
import { AppButton } from 'src/ui-common/button.model';

@Component({
    selector: 'app-text-field',
    templateUrl: './text-field.component.html',
    styleUrls: ['./text-field.component.scss']
})
export class TextFieldComponent
    extends TextComponent
    implements OnInit {
    @Input() readonly fieldTitle: string | null = null;
    @Input() readonly textFieldType: TextFieldType = defaultTextFieldType;
    @Input() readonly inputName: string = "text-field";
    @Input() readonly autocomplete: string | null = null;
    @Input() readonly placeholder: string | null = null;
    @Input() readonly appearance: 'fill' | 'outline' = 'fill';
    @Input() readonly suffixButton: AppButton | null = null;

    getExtraValidators(): ValidatorFn[] {
        return [
            Validators.maxLength(
                getTextFieldMaxLength(this.textFieldType),
            ),
        ];
    }
}
