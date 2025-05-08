import { NgModule } from "@angular/core";
import { AppRoutingModule } from "../app-routing.module";




import { LoginPageComponent } from "./auth/login/login-page/login-page.component";
import { RegisterPageComponent } from "./auth/register/register-page/register-page.component";

@NgModule({
    imports: [
    AppRoutingModule,
    RegisterPageComponent,
    LoginPageComponent,
],
    exports: [],
})
export class AppIdentityModule {

}
