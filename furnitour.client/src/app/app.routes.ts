import { Routes } from '@angular/router';
import { UserComponent } from './user/user.component';
import { AuthGuard } from './services/app.auth.guard';
import { SignOutComponent } from './sign-out/sign-out.component';
import { RegisterComponent } from './register/register.component';
import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { AdminComponent } from './admin/admin.component';
import { ForbiddenComponent } from './forbidden/forbidden.component';
export const routes: Routes = [
    {
        path: 'admin',
        component: AdminComponent,
        canActivate: [AuthGuard],
        data: { roles: ['Administrator'] }
      },
    {path: 'user', component: UserComponent, canActivate: [AuthGuard], data: { roles: ['Administrator', 'Master','User'] }},
    {path: 'logout', component: SignOutComponent},
    {path: 'register', component: RegisterComponent},
    {path: 'login', component: LoginComponent},
    {path: 'forbidden', component: ForbiddenComponent}
];
