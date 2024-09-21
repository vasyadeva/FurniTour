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
                this.status.isSignedIn = true;
                this.authService.getUserRole().subscribe(
                    (response : any )=> {
                        switch(response){
                            case "Administrator":
                                this.status.isAdmin = true;
                                break;
                            case "Master":
                                this.status.isMaster = true;
                                break;
                            case "User":
                                this.status.isUser = true;
                                break;
                            default:
                                break;
                        }
                    });
                this.router.navigateByUrl('')
              }
              console.log(response);
          },
          error => {
              if (!error?.error?.isSuccess) {
                  this.authFailed = true;
                  this.error = error?.error?.message || 'An unexpected error occurred. Please try again later.';
              }

          });
  }
  public logout() {
        this.authService.signOut().subscribe(
            () => {
                this.router.navigateByUrl('logout');
            });
  }
  public register() {
        this.router.navigateByUrl('register');
    }
}