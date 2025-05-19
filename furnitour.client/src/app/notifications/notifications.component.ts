import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { NotificationService, Notification } from '../services/notification.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './notifications.component.html',
  styleUrl: './notifications.component.css'
})
export class NotificationsComponent implements OnInit, OnDestroy {
  notifications: Notification[] = [];
  loading: boolean = true;
  currentPage: number = 1;
  pageSize: number = 10;
  private subscriptions: Subscription[] = [];

  constructor(private notificationService: NotificationService) {}

  ngOnInit(): void {
    this.loadNotifications();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  loadNotifications(): void {
    this.loading = true;
    this.subscriptions.push(
      this.notificationService.getNotifications(this.currentPage, this.pageSize).subscribe(notifications => {
        this.notifications = notifications;
        this.loading = false;
      })
    );
  }

  loadMore(): void {
    this.currentPage++;
    this.loading = true;
    this.subscriptions.push(
      this.notificationService.getNotifications(this.currentPage, this.pageSize).subscribe(newNotifications => {
        this.notifications = [...this.notifications, ...newNotifications];
        this.loading = false;
      })
    );
  }

  markAsRead(notificationId: number): void {
    this.notificationService.markAsRead(notificationId).subscribe();
  }

  deleteNotification(notificationId: number, event: Event): void {
    event.stopPropagation(); // Prevent triggering markAsRead
    this.notificationService.deleteNotification(notificationId).subscribe(() => {
      this.notifications = this.notifications.filter(n => n.id !== notificationId);
    });
  }

  markAllAsRead(): void {
    this.notificationService.markAllAsRead().subscribe(() => {
      this.notifications = this.notifications.map(n => ({...n, isRead: true}));
    });
  }
  
  hasUnreadNotifications(): boolean {
    return this.notifications.some(n => !n.isRead);
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

  getNotificationTypeText(type: string): string {
    switch (type) {
      case 'Order':
        return 'Замовлення';
      case 'IndividualOrder':
        return 'Індивідуальне замовлення';
      case 'Guarantee':
        return 'Гарантія';
      case 'System':
        return 'Системне';
      default:
        return 'Інше';
    }
  }
} 