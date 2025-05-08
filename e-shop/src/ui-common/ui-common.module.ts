import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { LayoutModule } from '@angular/cdk/layout';
import { MatDialogModule } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { ClipboardModule } from '@angular/cdk/clipboard';

import { PageNotFoundComponent } from "./page-not-found/page-not-found.component";
import { BrandLogoComponent } from "./brand-logo/brand-logo.component";

@NgModule({
    declarations: [
        BrandLogoComponent,
        PageNotFoundComponent,
    ],
    imports: [
        CommonModule,

        LayoutModule,
        MatDialogModule,
        MatTooltipModule,
        MatCardModule,
        MatInputModule,
        MatButtonModule,
        MatDatepickerModule,
        MatNativeDateModule,
        ClipboardModule,
    ],
    exports: [
        CommonModule,

        LayoutModule,
        MatDialogModule,
        MatTooltipModule,
        MatCardModule,
        MatInputModule,
        MatButtonModule,
        MatDatepickerModule,
        ClipboardModule,

        BrandLogoComponent,
    ]
})
export class UICommonModule {

}
