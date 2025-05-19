import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth/auth.service';
import { Router } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AppStatusService } from '../../services/auth/app.status.service'; 

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
    username: "Ім'я користувача є обов'язковим.",
    password: 'Пароль повинен містити мінімум 8 символів, включаючи літери та цифри.',
    confirmPassword: 'Підтвердіть пароль.',
    mismatch: 'Паролі не співпадають.',
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
        Validators.pattern(/^(?=.*[a-zA-Z])(?=.*\d)(?=.*[\W_])[a-zA-Z\d\W_]*$/)
      ]],
      confirmPassword: ['', Validators.required],
      isMaster: [false]
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
      const { username, password, isMaster } = this.registerForm.value;
      this.authService.register(username, password, isMaster).subscribe(
        () => { 
          this.status.updateAuthStatus({
            isSignedIn: true,
            isAdmin: false,
            isMaster: false,
            isUser: true
          });
          this.router.navigate(['/login']);
        }, 
        error => {
          console.error('Registration error:', error);
          this.registerFailed = true;
          this.errorRegister = error?.error?.message || 'Сталася неочікувана помилка. Будь ласка, спробуйте пізніше.';
        }
      );
    }
  }
}
