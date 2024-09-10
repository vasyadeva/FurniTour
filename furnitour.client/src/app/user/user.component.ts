import { Component } from '@angular/core';
import { UserClaim } from '../auth.interface';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [],
  templateUrl: './user.component.html',
  styleUrl: './user.component.css'
})
export class UserComponent {
  userClaims: UserClaim[] = [];
  constructor(private authService: AuthService) {
      this.getUser();
  }

  getUser() {
      this.authService.user().subscribe(
          result => {
              this.userClaims = result;
          });
  }
}