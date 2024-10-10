import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ProfileChangeModel } from '../../../models/profile.change.model';
import { AuthService } from '../../../services/auth/auth.service';
import { PopupService } from '../../../services/popup/popup.service';

@Component({
  selector: 'app-change-profile',
  standalone: true,
  imports: [FormsModule,CommonModule,ReactiveFormsModule],
  templateUrl: './change-profile.component.html',
  styleUrl: './change-profile.component.css'
})
export class ChangeProfileComponent {
  profile: ProfileChangeModel = {
    email: '',
    phonenumber: ''
  }
  form : FormGroup;
  constructor(private fb: FormBuilder,private authService: AuthService, private popupService: PopupService) {
      this.form = this.fb.group({
        email: [''],
        phonenumber: ['']
      });
   }

  submitProfile(): void {
    if (this.form.invalid) {
      return;
    }
    this.popupService.loadingSnackBar();
    this.profile  = this.form.value;
    this.authService.changeProfile(this.profile).subscribe(
      (response) => {
        this.popupService.closeSnackBar();
        this.popupService.openSnackBar('Profile updated successfully');
      },
      (error) => {
        this.popupService.closeSnackBar();
        this.popupService.openSnackBar(error?.error || 'Error updating profile');
        console.error('Error updating profile:', error);
      }
    );
  }
}
