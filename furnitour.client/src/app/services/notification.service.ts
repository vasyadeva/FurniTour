import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { BehaviorSubject, Observable, from, tap } from 'rxjs';
import { api} from '../../environments/app.environment';

export interface Notification {
  id: number;
  title: string;
  message: string;
  createdAt: Date;
  isRead: boolean;
  notificationType: string;
  orderId?: number;
  individualOrderId?: number;
  guaranteeId?: number;
  redirectUrl?: string;
}

export interface NotificationCount {
  totalCount: number;
  unreadCount: number;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private baseUrl = `${api}/notification`;
  private hubUrl = `${api.replace('/api', '')}/notificationHub`;
  private hubConnection!: HubConnection;
  private notificationsSource = new BehaviorSubject<Notification[]>([]);
  notifications$ = this.notificationsSource.asObservable();
  private unreadNotificationsSource = new BehaviorSubject<Notification[]>([]);
  unreadNotifications$ = this.unreadNotificationsSource.asObservable();
  private notificationCountsSource = new BehaviorSubject<NotificationCount>({ totalCount: 0, unreadCount: 0 });
  notificationCounts$ = this.notificationCountsSource.asObservable();
  private connectionIsEstablished = false;
  private visibilityChangeListener: any;

  constructor(private http: HttpClient) {
    // Track visibility changes to reconnect when tab becomes visible
    this.setupVisibilityChangeListener();
  }

  private setupVisibilityChangeListener(): void {
    this.visibilityChangeListener = () => {
      if (document.visibilityState === 'visible') {
        console.log('Tab became visible, checking notification connection...');
        this.ensureConnection();
      }
    };
    
    document.addEventListener('visibilitychange', this.visibilityChangeListener);
  }

  private ensureConnection(): void {
    if (!this.connectionIsEstablished) {
      console.log('Notification connection lost, reconnecting...');
      this.startConnection().subscribe({
        error: err => console.error('Failed to reconnect to notification hub:', err)
      });
    } else {
      // Even if the connection is established, refresh notification counts
      this.getNotificationCounts();
    }
  }

  public startConnection(): Observable<void> {
    if (this.connectionIsEstablished) {
      return from(Promise.resolve());
    }

    console.log('Starting Notification SignalR connection...');
    
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl, {
        withCredentials: true
      })
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect([0, 2000, 5000, 10000, 15000, 30000])
      .build();

    this.setupSignalRListeners();
    
    this.hubConnection.onreconnecting(error => {
      console.log('Notification connection reconnecting:', error);
      this.connectionIsEstablished = false;
    });
    
    this.hubConnection.onreconnected(() => {
      console.log('Notification connection reestablished');
      this.connectionIsEstablished = true;
      this.getNotificationCounts();
    });
    
    this.hubConnection.onclose(error => {
      console.log('Notification connection closed:', error);
      this.connectionIsEstablished = false;
      // Try to reconnect after a short delay
      setTimeout(() => this.ensureConnection(), 5000);
    });

    return from(this.hubConnection.start())
      .pipe(
        tap(() => {
          this.connectionIsEstablished = true;
          console.log('Notification Hub Connection started successfully');
          
          // After connection is established, we don't need to call anything 
          // as the hub will automatically send initial data in OnConnectedAsync
        })
      );
  }

  public stopConnection(): Observable<void> {
    if (!this.connectionIsEstablished) {
      return from(Promise.resolve());
    }

    // Remove visibility change listener when stopping
    document.removeEventListener('visibilitychange', this.visibilityChangeListener);

    return from(this.hubConnection.stop())
      .pipe(
        tap(() => {
          this.connectionIsEstablished = false;
          console.log('Notification Hub Connection stopped');
        })
      );
  }

  private setupSignalRListeners(): void {
    this.hubConnection.on('NewNotification', (notification: Notification) => {
      console.log('Received new notification:', notification);
      
      // Add to unread notifications
      const currentUnread = this.unreadNotificationsSource.value;
      this.unreadNotificationsSource.next([notification, ...currentUnread]);
      
      // Update all notifications if we have them loaded
      const currentAll = this.notificationsSource.value;
      if (currentAll.length > 0) {
        this.notificationsSource.next([notification, ...currentAll]);
      }
    });

    this.hubConnection.on('NotificationCounts', (counts: NotificationCount) => {
      console.log('Notification counts updated:', counts);
      this.notificationCountsSource.next(counts);
    });

    this.hubConnection.on('UnreadNotifications', (notifications: Notification[]) => {
      console.log('Received unread notifications:', notifications);
      this.unreadNotificationsSource.next(notifications);
    });
    
    this.hubConnection.on('ReceiveNotifications', (notifications: Notification[]) => {
      console.log('Received all notifications:', notifications);
      this.notificationsSource.next(notifications);
    });
  }

  // API methods
  public getNotifications(page: number = 1, pageSize: number = 20): Observable<Notification[]> {
    if (this.connectionIsEstablished) {
      // Use SignalR to get notifications if connection is established
      this.hubConnection.invoke('GetNotifications', page, pageSize);
      return this.notifications$;
    } else {
      // Fallback to HTTP API
      return this.http.get<Notification[]>(`${this.baseUrl}?page=${page}&pageSize=${pageSize}`, { withCredentials: true })
        .pipe(
          tap(notifications => this.notificationsSource.next(notifications))
        );
    }
  }

  public getUnreadNotifications(): Observable<Notification[]> {
    return this.http.get<Notification[]>(`${this.baseUrl}/unread`, { withCredentials: true })
      .pipe(
        tap(notifications => this.unreadNotificationsSource.next(notifications))
      );
  }

  public getNotificationCounts(): Observable<NotificationCount> {
    return this.http.get<NotificationCount>(`${this.baseUrl}/counts`, { withCredentials: true })
      .pipe(
        tap(counts => this.notificationCountsSource.next(counts))
      );
  }

  public markAsRead(notificationId: number): Observable<any> {
    if (this.connectionIsEstablished) {
      // Use SignalR to mark as read if connection is established
      this.hubConnection.invoke('MarkAsRead', notificationId);
    }
    
    // Always use HTTP API to ensure persistence
    return this.http.post(`${this.baseUrl}/mark-read/${notificationId}`, {}, { withCredentials: true })
      .pipe(
        tap(() => {
          // Update notifications after marking as read
          this.updateNotificationsAfterRead(notificationId);
        })
      );
  }

  public markAllAsRead(): Observable<any> {
    if (this.connectionIsEstablished) {
      // Use SignalR to mark all as read if connection is established
      this.hubConnection.invoke('MarkAllAsRead');
    }
    
    // Always use HTTP API to ensure persistence
    return this.http.post(`${this.baseUrl}/mark-all-read`, {}, { withCredentials: true })
      .pipe(
        tap(() => {
          // Mark all notifications as read locally
          const allNotifications = this.notificationsSource.value.map(n => ({...n, isRead: true}));
          this.notificationsSource.next(allNotifications);
          this.unreadNotificationsSource.next([]);
          this.notificationCountsSource.next({...this.notificationCountsSource.value, unreadCount: 0});
        })
      );
  }

  public deleteNotification(notificationId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${notificationId}`, { withCredentials: true })
      .pipe(
        tap(() => {
          // Remove notification from local collections
          const currentAll = this.notificationsSource.value;
          const currentUnread = this.unreadNotificationsSource.value;
          
          this.notificationsSource.next(currentAll.filter(n => n.id !== notificationId));
          this.unreadNotificationsSource.next(currentUnread.filter(n => n.id !== notificationId));
          
          // Update counts
          this.getNotificationCounts().subscribe();
        })
      );
  }
  
  private updateNotificationsAfterRead(notificationId: number): void {
    // Update the notification in the unread list
    const currentUnread = this.unreadNotificationsSource.value;
    const updatedUnread = currentUnread.filter(n => n.id !== notificationId);
    this.unreadNotificationsSource.next(updatedUnread);
    
    // Update the notification in the all notifications list
    const currentAll = this.notificationsSource.value;
    const updatedAll = currentAll.map(n => 
      n.id === notificationId ? {...n, isRead: true} : n
    );
    this.notificationsSource.next(updatedAll);
    
    // Decrement unread count
    const currentCounts = this.notificationCountsSource.value;
    if (currentCounts.unreadCount > 0) {
      this.notificationCountsSource.next({
        ...currentCounts,
        unreadCount: currentCounts.unreadCount - 1
      });
    }
  }
} 