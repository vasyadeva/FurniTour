import { Component } from '@angular/core';
import { AuthService } from '../../services/auth/auth.service';
import { AppStatusService } from '../../services/auth/app.status.service';
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [],
  templateUrl: './logout.component.html',
  styleUrl: './logout.component.css'
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
      this.status.isUser = false;
  }
}