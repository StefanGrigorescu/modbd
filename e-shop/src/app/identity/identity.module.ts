import { NgModule } from "@angular/core";
import { AppRoutingModule } from "../app-routing.module";

import { UICommonModule } from "src/ui-common/ui-common.module";
import { AppFormsModule } from "@forms/app-forms.module";

import { LoginPageComponent } from "./auth/login/login-page/login-page.component";
import { RegisterPageComponent } from "./auth/register/register-page/register-page.component";

@NgModule({
    declarations: [
        RegisterPageComponent,
        LoginPageComponent,
    ],
    imports: [
        AppRoutingModule,
        AppFormsModule,
        UICommonModule,
    ],
    exports: [],
})
export class AppIdentityModule {

}
