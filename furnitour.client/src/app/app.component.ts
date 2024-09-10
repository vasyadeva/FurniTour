import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms'; 
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { RegisterComponent } from "./register/register.component";
import { AppStatusService } from './services/app.status.service';
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
        this.authService.isSignedIn().subscribe(
            isSignedIn => {
                this.status.isSignedIn = isSignedIn;
            });
    }
}