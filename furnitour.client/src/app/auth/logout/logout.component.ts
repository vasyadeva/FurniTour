import { Component } from '@angular/core';
import { AuthService } from '../../services/auth/auth.service';
import { AppStatusService } from '../../services/auth/app.status.service';
import { Router } from '@angular/router';
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [],
  templateUrl: './logout.component.html',
  styleUrl: './logout.component.css'
})
export class SignOutComponent {
  constructor(private authService: AuthService, private status: AppStatusService,private router: Router) {
      this.signout();
  }
  public signout() {
      this.authService.signOut().subscribe(
        data => {
          this.status.signOut();
          this.router.navigate(['/login']);
        },
        error => {
        }
      );
      
  }
}