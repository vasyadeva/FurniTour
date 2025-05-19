import { Component, OnInit, OnDestroy } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth/auth.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms'; 
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AppStatusService } from './services/auth/app.status.service';
import { ChatService } from './services/chat.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, ReactiveFormsModule, CommonModule, RouterModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit, OnDestroy {
  unreadCount: number = 0;
  private subscriptions: Subscription[] = [];
  
  constructor(
    private authService: AuthService,
    private formBuilder: FormBuilder,
    private router: Router,
    public status: AppStatusService,
    private chatService: ChatService
  ) {
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
        
        // Start the chat service connection when user is signed in
        this.chatService.startConnection().subscribe();
      } else {
        //this.router.navigate(['login']);
      }
    });
  }
  
  ngOnInit(): void {
    // Subscribe to unread message count updates
    this.subscriptions.push(
      this.chatService.unreadCount$.subscribe(count => {
        this.unreadCount = count;
      })
    );
  }
  
  ngOnDestroy(): void {
    // Clean up subscriptions when component is destroyed
    this.subscriptions.forEach(sub => sub.unsubscribe());
    this.chatService.stopConnection().subscribe();
  }
}