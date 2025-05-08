import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { UICommonModule } from 'src/ui-common/ui-common.module';
import { AppFormsModule } from '@forms/app-forms.module';
import { AppIdentityModule } from '@identity/identity.module';
import { AppComponent } from './app.component';

@NgModule({
    declarations: [
        AppComponent,
    ],
    imports: [
        BrowserModule,
        NoopAnimationsModule,
        CommonModule,
        HttpClientModule,

        AppRoutingModule,
        UICommonModule,
        AppFormsModule,
        AppIdentityModule,
    ],
    providers: [],
    bootstrap: [AppComponent]
})
export class AppModule { }
