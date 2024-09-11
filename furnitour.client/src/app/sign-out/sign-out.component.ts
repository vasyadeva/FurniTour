import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { AppStatusService } from '../services/app.status.service';
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [],
  templateUrl: './sign-out.component.html',
  styleUrl: './sign-out.component.css'
})
export class SignOutComponent {
  constructor(private authService: AuthService, private status: AppStatusService) {
      this.signout();
  }
  public signout() {
      this.authService.signOut().subscribe();
      this.status.isSignedIn = false;
      this.status.isAdmin = false;
      this.status.isMaster = false;
  }
}