import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth/auth.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms'; 
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AppStatusService } from './services/auth/app.status.service';
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, ReactiveFormsModule, CommonModule, RouterModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  
    constructor(private authService: AuthService,
        private formBuilder: FormBuilder,
        private router: Router,
        public status: AppStatusService) {
            this.authService.isSignedIn().subscribe(isSignedIn => {
                if (isSignedIn) {
                  this.status.isSignedIn = true;
                  this.authService.getUserRole().subscribe(role => {
                    console.log(role);
                    switch (role) {
                      case 'Administrator':
                        this.status.isAdmin = true;
                        break;
                      case 'Master':
                        this.status.isMaster = true;
                        break;
                      case 'User':
                        this.status.isUser = true;
                        break;
                      default:
                        break;
                    }
                  });
                } else {
                  //this.router.navigate(['login']);
                }
              });
        
    }
}