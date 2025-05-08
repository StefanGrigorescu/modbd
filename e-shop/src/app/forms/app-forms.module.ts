import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { TextFieldComponent } from "./text/text-field/text-field.component";
import { TextFieldCustomComponent } from "./text/text-field-custom/text-field-custom.component";
import { TextBoxComponent } from "./text/text-box/text-box.component";
import { EmailComponent } from "./email/email.component";
import { PasswordComponent } from "./passwords/password/password.component";
import { UICommonModule } from "src/ui-common/ui-common.module";

@NgModule({
    declarations: [
        TextFieldComponent,
        TextFieldCustomComponent,
        TextBoxComponent,
        EmailComponent,
        PasswordComponent,
    ],
    imports: [
        FormsModule,
        ReactiveFormsModule,

        UICommonModule,
    ],
    exports: [
        FormsModule,
        ReactiveFormsModule,

        TextFieldComponent,
        TextFieldCustomComponent,
        TextBoxComponent,
        EmailComponent,
        PasswordComponent,
    ]
})
export class AppFormsModule {

}
