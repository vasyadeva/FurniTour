import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthService } from '../../services/auth/auth.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms'; 
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { RegisterComponent } from "../register/register.component";
import { AppStatusService } from '../../services/auth/app.status.service';
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, ReactiveFormsModule, CommonModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  authFailed: boolean = false;
  signedIn: boolean = false;
  error: string = '';
  constructor(private authService: AuthService,
      private formBuilder: FormBuilder,
      private router: Router,
      public status: AppStatusService) {
      this.authService.isSignedIn().subscribe(
          isSignedIn => {
              this.signedIn = isSignedIn;
          });
  }
  ngOnInit(): void {
      this.authFailed = false;
      this.loginForm = this.formBuilder.group(
          {
              username: ['', [Validators.required]],
              password: ['', [Validators.required]]
          });
  }
  public signIn(event: any) {
      const userName = this.loginForm.get('username')?.value;
      const password = this.loginForm.get('password')?.value;
      this.authService.signIn(userName, password).subscribe(
          (response : any)=> {
              if (response.isSuccess) {
                // Create new auth status object with signed in set to true
                const authStatus = {
                  isSignedIn: true,
                  isAdmin: false,
                  isMaster: false,
                  isUser: false
                };
                
                this.authService.getUserRole().subscribe(
                    (response : any )=> {
                        switch(response){
                            case "Administrator":
                                authStatus.isAdmin = true;
                                break;
                            case "Master":
                                authStatus.isMaster = true;
                                break;
                            case "User":
                                authStatus.isUser = true;
                                break;
                            default:
                                break;
                        }
                        // Update the auth status with the complete object
                        this.status.updateAuthStatus(authStatus);
                    });
                this.router.navigateByUrl('')
              }
              console.log(response);
          },
          error => {
              if (!error?.error?.isSuccess) {
                  this.authFailed = true;
                  this.error = error?.error?.message || 'Сталася неочікувана помилка. Будь ласка, спробуйте пізніше.';
              }

          });
  }
  public logout() {
        this.authService.signOut().subscribe(
            () => {
                this.status.signOut();
                this.router.navigateByUrl('logout');
            });
  }
  public register() {
        this.router.navigateByUrl('register');
    }
}