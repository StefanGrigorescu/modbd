import { Component, Input, OnInit } from '@angular/core';
import { ValidatorFn, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { emailMaxLength } from '../text/text-max-lengths.const';
import { TextComponent } from '../text/text-component.model';
import { NgIf } from '@angular/common';
import { MatInput } from '@angular/material/input';
import { MatFormField, MatLabel, MatError } from '@angular/material/form-field';

@Component({
    selector: 'app-email',
    templateUrl: './email.component.html',
    styleUrls: ['./email.component.scss'],
    standalone: true,
    imports: [MatFormField, MatLabel, MatInput, FormsModule, ReactiveFormsModule, NgIf, MatError]
})
export class EmailComponent
    extends TextComponent
    implements OnInit {
    @Input() readonly fieldTitle: string = "Email";
    @Input() readonly appearance: 'fill' | 'outline' = 'fill';

    getExtraValidators(): ValidatorFn[] {
        return [
            Validators.email,
            Validators.maxLength(emailMaxLength),
        ];
    }
}
