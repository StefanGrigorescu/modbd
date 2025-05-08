import { Component, Input, OnInit } from '@angular/core';
import { ValidatorFn, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TextBoxType, defaultTextBoxType, getTextBoxMaxLength } from '../text-box-type.enum';
import { TextComponent } from '../text-component.model';
import { MatInput } from '@angular/material/input';
import { NgIf } from '@angular/common';
import { MatFormField, MatLabel, MatError } from '@angular/material/form-field';

@Component({
    selector: 'app-text-box',
    templateUrl: './text-box.component.html',
    styleUrls: ['./text-box.component.scss'],
    standalone: true,
    imports: [MatFormField, NgIf, MatLabel, MatInput, FormsModule, ReactiveFormsModule, MatError]
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
