import { Component, Input, OnInit } from '@angular/core';
import { ValidatorFn, Validators } from '@angular/forms';
import { emailMaxLength } from '../text/text-max-lengths.const';
import { TextComponent } from '../text/text-component.model';

@Component({
    selector: 'app-email',
    templateUrl: './email.component.html',
    styleUrls: ['./email.component.scss']
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
