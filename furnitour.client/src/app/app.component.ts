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
import { NotificationService, Notification, NotificationCount } from './services/notification.service';
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
  unreadNotificationCount: number = 0;
  unreadNotifications: Notification[] = [];
  private subscriptions: Subscription[] = [];
  showNotificationsDropdown: boolean = false;
  
  constructor(
    private authService: AuthService,
    private formBuilder: FormBuilder,
    private router: Router,
    public status: AppStatusService,
    private chatService: ChatService,
    private notificationService: NotificationService
  ) {
    this.authService.isSignedIn().subscribe(isSignedIn => {
      if (isSignedIn) {
        // Create new auth status object with signed in set to true
        const authStatus = {
          isSignedIn: true,
          isAdmin: false,
          isMaster: false,
          isUser: false
        };
        
        this.authService.getUserRole().subscribe(role => {
          console.log(role);
          switch (role) {
            case 'Administrator':
              authStatus.isAdmin = true;
              break;
            case 'Master':
              authStatus.isMaster = true;
              break;
            case 'User':
              authStatus.isUser = true;
              break;
            default:
              break;
          }
          
          // Update the auth status with the complete object
          this.status.updateAuthStatus(authStatus);
        });
        
        // Start the chat service connection when user is signed in
        this.chatService.startConnection().subscribe();
        
        // Start the notification service connection when user is signed in
        this.notificationService.startConnection().subscribe();
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
    
    // Subscribe to notification counts
    this.subscriptions.push(
      this.notificationService.notificationCounts$.subscribe(counts => {
        this.unreadNotificationCount = counts.unreadCount;
      })
    );
    
    // Subscribe to unread notifications
    this.subscriptions.push(
      this.notificationService.unreadNotifications$.subscribe(notifications => {
        this.unreadNotifications = notifications;
      })
    );
  }
  
  ngOnDestroy(): void {
    // Clean up subscriptions when component is destroyed
    this.subscriptions.forEach(sub => sub.unsubscribe());
    this.chatService.stopConnection().subscribe();
    this.notificationService.stopConnection().subscribe();
  }
  
  toggleNotificationsDropdown(): void {
    this.showNotificationsDropdown = !this.showNotificationsDropdown;
  }
  
  markNotificationAsRead(notificationId: number): void {
    this.notificationService.markAsRead(notificationId).subscribe();
    
    // If the notification has a redirect URL, navigate to it
    const notification = this.unreadNotifications.find(n => n.id === notificationId);
    if (notification && notification.redirectUrl) {
      this.router.navigateByUrl(notification.redirectUrl);
      this.showNotificationsDropdown = false;
    }
  }
  
  markAllNotificationsAsRead(): void {
    this.notificationService.markAllAsRead().subscribe();
    this.showNotificationsDropdown = false;
  }
  
  getNotificationIcon(type: string): string {
    switch (type) {
      case 'Order':
        return 'fa fa-shopping-cart';
      case 'IndividualOrder':
        return 'fa fa-hammer';
      case 'Guarantee':
        return 'fa fa-shield-alt';
      case 'System':
        return 'fa fa-bell';
      default:
        return 'fa fa-bell';
    }
  }
}