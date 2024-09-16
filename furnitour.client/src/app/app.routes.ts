import { Routes } from '@angular/router';
import { UserComponent } from './user/user.component';
import { AuthGuard } from './services/app.auth.guard';
import { SignOutComponent } from './logout/logout.component';
import { RegisterComponent } from './register/register.component';
import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { AdminComponent } from './admin/admin.component';
import { ForbiddenComponent } from './forbidden/forbidden.component';
import { HomeComponent } from './home/home.component';
export const routes: Routes = [
    {
        path: 'admin',
        component: AdminComponent,
        canActivate: [AuthGuard],
        data: { roles: ['Administrator'] }
      },
    {path: 'user', component: UserComponent},//, canActivate: [AuthGuard], data: { roles: ['Administrator'] }},
    {path: 'logout', component: SignOutComponent},
    {path: 'register', component: RegisterComponent},
    {path: 'login', component: LoginComponent},
    {path: 'forbidden', component: ForbiddenComponent},
    {path: 'home', component: HomeComponent},
    {path: '', redirectTo: '/home', pathMatch: 'full'}
];
