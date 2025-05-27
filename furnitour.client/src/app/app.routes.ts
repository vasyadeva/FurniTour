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
import { ItemEditComponent } from './item/item-edit/item-edit.component';
import { ManufacturersComponent } from './manufacturer/manufacturers/manufacturers.component';
import { UpdateManufacturerComponent } from './manufacturer/update-manufacturer/update-manufacturer.component';
import { InfoManufacturerComponent } from './manufacturer/info-manufacturer/info-manufacturer.component';
import { AddManufacturerComponent } from './manufacturer/add-manufacturer/add-manufacturer.component';
import { MasterprofileComponent } from './profile/MasterProfile/masterprofile/masterprofile.component';
import { MasterAddReviewComponent } from './profile/MasterAddReview/master-add-review/master-add-review.component';
import { ManufacturerProfileComponent } from './profile/ManufacturerProfile/manufacturer-profile/manufacturer-profile.component';
import { ManufacturerAddReviewComponent } from './profile/ManufacturerAddReview/manufacturer-add-review/manufacturer-add-review.component';
import { ProfileComponent } from './auth/profile/profile/profile.component';
import { ChangeProfileComponent } from './auth/profile/change-profile/change-profile.component';
import { CreateIndividualOrderComponent } from './individual-order/create-individual-order/create-individual-order.component';
import { MyIndividualOrdersComponent } from './individual-order/my-individual-orders/my-individual-orders.component';
import { IndividualOrderDetailsComponent } from './individual-order/individual-order-details/individual-order-details.component';
import { AdminIndividualOrdersComponent } from './individual-order/admin-individual-orders/admin-individual-orders.component';
import { GuaranteeListComponent } from './guarantee/guarantee-list/guarantee-list.component';
import { GuaranteeDetailComponent } from './guarantee/guarantee-detail/guarantee-detail.component';
import { GuaranteeCreateComponent } from './guarantee/guarantee-create/guarantee-create.component';
import { GuaranteeAdminComponent } from './guarantee/guarantee-admin/guarantee-admin.component';
import { ChatDashboardComponent } from './chat/chat-dashboard/chat-dashboard.component';
import { UserManagementComponent } from './admin/user-management/user-management.component';
import { ColorManagementComponent } from './admin/color-management/color-management.component';
import { CategoryManagementComponent } from './admin/category-management/category-management.component';

export const routes: Routes = [
    //login paths
    {path: 'admin',component: AdminComponent,canActivate: [AuthGuard],data: { roles: ['Administrator'] }},
    {path: 'user', component: UserComponent},
    {path: 'logout', component: SignOutComponent},
    {path: 'register', component: RegisterComponent},
    {path: 'login', component: LoginComponent},    {path: 'forbidden', component: ForbiddenComponent},
    {path: 'profile', component: ProfileComponent},
    {path: 'profile/user/:username', component: ProfileComponent},
    {path: 'changeprofile', component: ChangeProfileComponent},

    {path: 'home', component: HomeComponent},
    {path: '', redirectTo: '/home', pathMatch: 'full'},
    
    //items paths
    {path: 'create', component: CreateItemComponent},
    {path: 'items', component: ItemsComponent},
    {path: 'details/:id', component: ItemInfoComponent},
    {path: 'edit/:id', component: ItemEditComponent},

    //cart paths
    {path: 'cart', component: CartComponent},

    //order paths
    {path: 'order', component: OrderComponent},
    {path: 'myorders', component: MyordersComponent},
    {path: 'adminorders', component: AdminOrdersComponent},

    //manufacturer paths
    {path: 'manufacturers', component: ManufacturersComponent},
    {path: 'manufacturers/update/:id', component: UpdateManufacturerComponent},
    {path: 'manufacturers/details/:id', component: InfoManufacturerComponent},
    {path: 'manufacturers/add', component: AddManufacturerComponent},
    
    //profile paths
    {path: 'profile/master/:id', component: MasterprofileComponent},
    {path: 'profile/manufacturer/:id', component: ManufacturerProfileComponent},
    {path: 'masterreview/add/:id', component: MasterAddReviewComponent},
    {path: 'manufacturerreview/add/:id', component: ManufacturerAddReviewComponent},
    
    //individual order paths
    {path: 'create-individual-order', component: CreateIndividualOrderComponent, canActivate: [AuthGuard],data: { roles: ['User'] }},
    {path: 'individual-orders', component: MyIndividualOrdersComponent, canActivate: [AuthGuard],data: { roles: ['User'] }},
    {path: 'individual-order/:id', component: IndividualOrderDetailsComponent, canActivate: [AuthGuard],data: { roles: ['User','Administrator', 'Master'] }},
    {path: 'admin-individual-orders', component: AdminIndividualOrdersComponent, canActivate: [AuthGuard], data: { roles: ['Administrator', 'Master'] }},
    
    //guarantee paths
    {path: 'guarantees/create', component: GuaranteeCreateComponent, canActivate: [AuthGuard], data: { roles: ['User'] }},
    {path: 'guarantees/:id', component: GuaranteeDetailComponent, canActivate: [AuthGuard], data: { roles: ['User', 'Administrator'] }},
    {path: 'guarantees', component: GuaranteeListComponent, canActivate: [AuthGuard], data: { roles: ['User', 'Administrator'] }},
    {path: 'admin/guarantees', component: GuaranteeAdminComponent, canActivate: [AuthGuard], data: { roles: ['Administrator'] }},
      //admin management paths
    {path: 'admin/users', component: UserManagementComponent, canActivate: [AuthGuard], data: { roles: ['Administrator'] }},
    {path: 'admin/colors', component: ColorManagementComponent, canActivate: [AuthGuard], data: { roles: ['Administrator'] }},
    {path: 'admin/categories', component: CategoryManagementComponent, canActivate: [AuthGuard], data: { roles: ['Administrator'] }},
    
    //chat path
    {path: 'chat', component: ChatDashboardComponent, canActivate: [AuthGuard],data: { roles: ['User','Administrator', 'Master'] }},
    {
      path: 'notifications',
      loadComponent: () => import('./notifications/notifications.component').then(c => c.NotificationsComponent),
      canActivate: [AuthGuard],
      data: { roles: ['User', 'Administrator', 'Master'] }
    }
];