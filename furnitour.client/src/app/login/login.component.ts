import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthService } from '../auth.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms'; 
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { RegisterComponent } from "../register/register.component";
import { AppStatusService } from '../app.status.service';
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, ReactiveFormsModule, CommonModule, RouterModule, RegisterComponent],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  authFailed: boolean = false;
  signedIn: boolean = false;
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
      /*if (!this.loginForm.valid) {
          return;
      }*/
      const userName = this.loginForm.get('username')?.value;
      const password = this.loginForm.get('password')?.value;
      this.authService.signIn(userName, password).subscribe(
          (response : any)=> {
              if (response.isSuccess) {
                this.status.isSignedIn = true;
                  this.router.navigateByUrl('user')
              }
              console.log(response);
          },
          error => {
              if (!error?.error?.isSuccess) {
                  this.authFailed = true;
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