import { Component } from '@angular/core';
import { AuthService } from '../../../services/auth/auth.service';
import { PopupService } from '../../../services/popup/popup.service';
import { ProfileModel } from '../../../models/profile.model';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent {
  profile: ProfileModel = {
    username: '',
    email: '',
    phonenumber: '',
    role: ''
  }
  constructor(private authService: AuthService, private popupService: PopupService )
  {
    this.popupService.loadingSnackBar();
    this.authService.getProfile().subscribe(
      (response) => {
        this.popupService.closeSnackBar();
        this.profile = response;
      },
      (error) => {
        this.popupService.closeSnackBar();
        this.popupService.openSnackBar(error?.error || 'Error fetching profile');
        console.error('Error fetching profile:', error);
      }
    );
  }
}
