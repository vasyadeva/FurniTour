import { Routes } from '@angular/router';
import { UserComponent } from './user/user.component';
import { AuthGuard } from './app.auth.guard';
import { SignOutComponent } from './sign-out/sign-out.component';
import { RegisterComponent } from './register/register.component';
import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
export const routes: Routes = [
    {path: 'user', component: UserComponent, canActivate: [AuthGuard]},
    {path: 'logout', component: SignOutComponent},
    {path: 'register', component: RegisterComponent},
    {path: 'login', component: LoginComponent}
];
