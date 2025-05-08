import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AuthGuard } from '@identity/auth/auth.guard';
import { AlreadyLoggedInGuard } from '@identity/auth/already-logged-in.guard';
import { RegisterPageComponent } from '@identity/auth/register/register-page/register-page.component';
import { LoginPageComponent } from '@identity/auth/login/login-page/login-page.component';

import { PageNotFoundComponent } from 'src/ui-common/page-not-found/page-not-found.component';

const appRoutes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },

    { path: 'auth/logout', title: 'Logging out...', redirectTo: 'auth/login' },
    {
        path: 'auth/login',
        title: 'Login',
        canActivate: [AlreadyLoggedInGuard],
        component: LoginPageComponent,
    },
    {
        path: 'auth/register',
        title: 'Register',
        canActivate: [AlreadyLoggedInGuard],
        component: RegisterPageComponent,
    },

    {
        path: 'not-found',
        title: 'Not Found',
        component: PageNotFoundComponent,
    },

    { path: '**', redirectTo: '/not-found' },
    // the wildcard route ** for all routes: this must be the last route to not override known paths
    /* you can use another parameter in a object: the 'data' object, which can have any key-value pairs needed for the loaded component
     then, in the component, use
        activatedRoute.snapshot.data['myKey'] for values that do not change (static data) or
        activatedRoute.data.subscribe(data: Data) => {...} for values that might change (dynamic data); this case uses a Resolver
     */
];

@NgModule({
    imports: [RouterModule.forRoot(appRoutes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
