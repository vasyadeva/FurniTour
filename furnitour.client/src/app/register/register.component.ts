import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AppStatusService } from '../services/app.status.service'; 

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  registerForm: FormGroup;
  registerFailed: boolean = false;
  errorRegister: string = '';
  errorMessages: { [key: string]: string } = {
    username: 'Username is required.',
    password: 'Password must be at least 8 characters long and include both letters and numbers.',
    confirmPassword: 'Confirm Password is required.',
    mismatch: 'Passwords do not match.',
  };

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private status: AppStatusService
  ) {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', [
        Validators.required, 
        Validators.minLength(8), 
        Validators.pattern(/^(?=.*[a-zA-Z])(?=.*\d)[a-zA-Z\d]*$/) // At least one letter and one number
      ]],
      confirmPassword: ['', Validators.required],
    }, { 
      validator: this.passwordMatchValidator 
    });
  }

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password')?.value;
    const confirmPassword = form.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { mismatch: true };
  }

  get formControls() {
    return this.registerForm.controls;
  }

  registerUser() {
    if (this.registerForm.valid) {
      const { username, password } = this.registerForm.value;
      this.authService.register(username, password).subscribe(
        () => { 
          this.status.isSignedIn = true;
          this.router.navigate(['/login']);
        }, 
        error => {
          console.error('Registration error:', error);
          this.registerFailed = true;
          this.errorRegister = error?.error?.message || 'An unexpected error occurred. Please try again later.';
        }
      );
    }
  }
}
