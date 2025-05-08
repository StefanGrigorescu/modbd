import { Component, Input, OnInit } from '@angular/core';
import { ValidatorFn, Validators } from '@angular/forms';
import { TextBoxType, defaultTextBoxType, getTextBoxMaxLength } from '../text-box-type.enum';
import { TextComponent } from '../text-component.model';

@Component({
    selector: 'app-text-box',
    templateUrl: './text-box.component.html',
    styleUrls: ['./text-box.component.scss']
})
export class TextBoxComponent
    extends TextComponent
    implements OnInit {
    @Input() readonly fieldTitle: string | null = null;
    @Input() readonly textBoxType: TextBoxType = defaultTextBoxType;
    @Input() readonly inputName: string = "text-field";
    @Input() readonly autocomplete: string | null = null;

    getExtraValidators(): ValidatorFn[] {
        return [
            Validators.maxLength(
                getTextBoxMaxLength(this.textBoxType),
            ),
        ];
    }
}
