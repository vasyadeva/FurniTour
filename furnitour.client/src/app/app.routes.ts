import { Routes } from '@angular/router';
import { UserComponent } from './user/user.component';
import { AuthGuard } from './services/auth/app.auth.guard';
import { SignOutComponent } from './auth/logout/logout.component';
import { RegisterComponent } from './auth/register/register.component';
import { AppComponent } from './app.component';
import { LoginComponent } from './auth/login/login.component';
import { AdminComponent } from './auth/admin/admin.component';
import { ForbiddenComponent } from './forbidden/forbidden.component';
import { HomeComponent } from './home/home.component';
import { CreateItemComponent } from './item/create-item/create-item.component';
import { ItemsComponent } from './item/items/items.component';
import { ItemInfoComponent } from './item/item-info/item-info.component';
import { CartComponent } from './cart/cart/cart.component';
import { OrderComponent } from './order/order/order.component';
import { MyordersComponent } from './order/myorders/myorders.component';
import { AdminOrdersComponent } from './order/admin-orders/admin-orders.component';
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
    {path: '', redirectTo: '/home', pathMatch: 'full'},
    {path: 'create', component: CreateItemComponent},
    {path: 'items', component: ItemsComponent},
    {path: 'details/:id', component: ItemInfoComponent},
    {path: 'cart', component: CartComponent},
    {path: 'order', component: OrderComponent},
    {path: 'myorders', component: MyordersComponent},
    {path: 'adminorders', component: AdminOrdersComponent}
];
