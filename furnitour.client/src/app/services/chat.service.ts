import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { BehaviorSubject, Observable, from, tap, map } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Message {
  id: number;
  content: string;
  sentAt: Date;
  isRead: boolean;
  senderId: string;
  senderName: string;
  receiverId: string;
  receiverName: string;
  conversationId?: number;
  
  // Photo attachment properties
  hasPhoto?: boolean;
  photoContentType?: string;
  photoId?: number; // Same as message ID if photo exists
}

export interface Conversation {
  id: number;
  user1Id: string;
  user1Name: string;
  user2Id: string;
  user2Name: string;
  lastActivity: Date;
  lastMessage?: Message;
  unreadCount: number;
}

export interface UserOnline {
  userId: string;
  userName: string;
  role: string;
  isOnline: boolean;
}

export interface SendMessage {
  receiverId: string;
  content: string;
  
  // Photo attachment (optional)
  photoData?: File;  // This remains File type on the frontend
  photoContentType?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private baseUrl = `${environment.apiUrl}/api/chat`;
  private hubUrl = `${environment.apiUrl}/chatHub`;
  private hubConnection!: HubConnection;
  private messagesSource = new BehaviorSubject<Message[]>([]);
  messages$ = this.messagesSource.asObservable();
  private conversationsSource = new BehaviorSubject<Conversation[]>([]);
  conversations$ = this.conversationsSource.asObservable();
  private onlineUsersSource = new BehaviorSubject<UserOnline[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();
  private unreadCountSource = new BehaviorSubject<number>(0);
  unreadCount$ = this.unreadCountSource.asObservable();
  private connectionIsEstablished = false;
  private visibilityChangeListener: any;

  constructor(private http: HttpClient) {
    // Track visibility changes to reconnect when tab becomes visible
    this.setupVisibilityChangeListener();
  }

  private setupVisibilityChangeListener(): void {
    this.visibilityChangeListener = () => {
      if (document.visibilityState === 'visible') {
        console.log('Tab became visible, checking connection...');
        this.ensureConnection();
      }
    };
    
    document.addEventListener('visibilitychange', this.visibilityChangeListener);
  }

  private ensureConnection(): void {
    if (!this.connectionIsEstablished) {
      console.log('Connection lost, reconnecting...');
      this.startConnection().subscribe({
        error: err => console.error('Failed to reconnect:', err)
      });
    } else {
      // Even if the connection is established, refresh online users
      this.getOnlineUsers();
    }
  }

  public startConnection(): Observable<void> {
    if (this.connectionIsEstablished) {
      return from(Promise.resolve());
    }

    console.log('Starting SignalR connection...');
    
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl, {
        withCredentials: true
      })
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect([0, 2000, 5000, 10000, 15000, 30000]) // More aggressive reconnect strategy
      .build();

    this.setupSignalRListeners();
    
    this.hubConnection.onreconnecting(error => {
      console.log('Connection reconnecting:', error);
      this.connectionIsEstablished = false;
    });
    
    this.hubConnection.onreconnected(connectionId => {
      console.log('Connection reestablished. ID:', connectionId);
      this.connectionIsEstablished = true;
      this.getOnlineUsers();
      this.refreshUnreadCount();
    });
    
    this.hubConnection.onclose(error => {
      console.log('Connection closed:', error);
      this.connectionIsEstablished = false;
      // Try to reconnect after a short delay
      setTimeout(() => this.ensureConnection(), 5000);
    });

    return from(this.hubConnection.start())
      .pipe(
        tap(() => {
          this.connectionIsEstablished = true;
          console.log('Chat Hub Connection started successfully');
          this.getOnlineUsers();
          this.refreshUnreadCount();
          // Start heartbeat after connection is established
          this.startHeartbeat();
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
          console.log('Chat Hub Connection stopped');
        })
      );
  }

  private setupSignalRListeners(): void {
    this.hubConnection.on('ReceiveMessage', (message: Message) => {
      // Process message to check for photo-only messages
      if (this.isPhotoOnlyMessage(message)) {
        console.log('Received photo-only message');
      }
      
      const currentMessages = this.messagesSource.value;
      this.messagesSource.next([...currentMessages, message]);
      this.refreshConversations();
      this.refreshUnreadCount();
    });

    this.hubConnection.on('OnlineUsers', (users: UserOnline[]) => {
      console.log('Received online users update:', users);
      console.log('Online users count:', users.filter(u => u.isOnline).length);
      
      // For debugging, log which users are online
      const onlineUserNames = users.filter(u => u.isOnline).map(u => u.userName).join(', ');
      console.log('Online users:', onlineUserNames || 'None');
      
      this.onlineUsersSource.next(users);
    });

    this.hubConnection.on('UserOnline', (userId: string) => {
      console.log('User came online:', userId);
      const users = this.onlineUsersSource.value;
      
      // Check if user exists in the list
      const userExists = users.some(u => u.userId === userId);
      
      if (userExists) {
        // Update existing user
        const updatedUsers = users.map(user => 
          user.userId === userId ? {...user, isOnline: true} : user
        );
        console.log(`Updated user ${userId} to online status`);
        this.onlineUsersSource.next(updatedUsers);
      } else {
        // Refresh the entire list to get the new user
        console.log(`User ${userId} not found in current list, refreshing all users`);
        this.getOnlineUsers();
      }
    });

    this.hubConnection.on('UserOffline', (userId: string) => {
      console.log('User went offline:', userId);
      const users = this.onlineUsersSource.value;
      const updatedUsers = users.map(user => 
        user.userId === userId ? {...user, isOnline: false} : user
      );
      console.log(`Updated user ${userId} to offline status`);
      this.onlineUsersSource.next(updatedUsers);
    });

    this.hubConnection.on('MessagesRead', (conversationId: number, userId: string) => {
      const currentMessages = this.messagesSource.value;
      const updatedMessages = currentMessages.map(message => 
        message.conversationId === conversationId && message.receiverId === userId
          ? {...message, isRead: true}
          : message
      );
      this.messagesSource.next(updatedMessages);
    });

    this.hubConnection.on('UpdateUnreadCount', () => {
      this.refreshUnreadCount();
    });

    this.hubConnection.on('HeartbeatResponse', () => {
      console.log('Heartbeat response received from server');
    });
  }

  // Helper method to identify photo-only messages
  private isPhotoOnlyMessage(message: Message): boolean {
    return message.hasPhoto === true && message.content === 'Photo attachment';
  }

  // API methods
  public getConversations(): Observable<Conversation[]> {
    return this.http.get<Conversation[]>(`${this.baseUrl}/conversations`, { withCredentials: true })
      .pipe(
        tap(conversations => this.conversationsSource.next(conversations))
      );
  }

  public getConversation(id: number): Observable<Conversation> {
    return this.http.get<Conversation>(`${this.baseUrl}/conversation/${id}`, { withCredentials: true });
  }

  public getConversationWithUser(userId: string): Observable<Conversation> {
    return this.http.get<Conversation>(`${this.baseUrl}/conversation/user/${userId}`, { withCredentials: true });
  }

  public getMessages(conversationId: number, page: number = 1, pageSize: number = 20): Observable<Message[]> {
    return this.http.get<Message[]>(`${this.baseUrl}/messages/${conversationId}?page=${page}&pageSize=${pageSize}`, { withCredentials: true })
      .pipe(
        tap(messages => this.messagesSource.next(messages))
      );
  }

  public markAsRead(conversationId: number): Observable<any> {
    return this.http.post(`${this.baseUrl}/markAsRead/${conversationId}`, {}, { withCredentials: true })
      .pipe(
        tap(() => {
          if (this.connectionIsEstablished) {
            this.hubConnection.invoke('MarkAsRead', conversationId);
          }
          this.refreshUnreadCount();
        })
      );
  }

  public getOnlineUsers(): void {
    if (this.connectionIsEstablished) {
      console.log('Requesting online users from server...');
      this.hubConnection.invoke('GetOnlineUsers')
        .catch(err => console.error('Error invoking GetOnlineUsers:', err));
    } else {
      console.warn('Cannot get online users: connection not established');
    }
  }

  public sendMessage(message: SendMessage): void {
    if (this.connectionIsEstablished) {
      // If there's a photo attached, we need to handle it differently
      if (message.photoData) {
        this.sendPhotoViaApi(message);
      } else {
        this.hubConnection.invoke('SendMessage', message);
      }
    }
  }

  private sendPhotoViaApi(message: SendMessage): void {
    if (!message.photoData) {
      this.hubConnection.invoke('SendMessage', message);
      return;
    }

    // Ensure content is not empty - if it's default "Photo attachment", use a space
    let messageContent = message.content?.trim() || "Photo attachment";
    if (messageContent === "Photo attachment") {
      messageContent = " "; // Use a single space to prevent visible text
    }

    // Create a FormData object to send the file
    const formData = new FormData();
    formData.append('file', message.photoData);
    formData.append('receiverId', message.receiverId);
    formData.append('content', messageContent);

    console.log('Sending photo via API. File size:', message.photoData.size);
    console.log('Message content:', messageContent);

    // Use HttpClient to send the FormData
    this.http.post<Message>(`${this.baseUrl}/upload-photo`, formData, { 
      withCredentials: true 
    }).subscribe({
      next: (response) => {
        console.log('Photo uploaded successfully:', response);
        // Add the message to the list
        const currentMessages = this.messagesSource.value;
        this.messagesSource.next([...currentMessages, response]);
        
        // Update the conversation list
        this.refreshConversations();
        
        // Notify the recipient via SignalR that they have a new message
        if (this.connectionIsEstablished) {
          this.hubConnection.invoke('NotifyNewMessage', response.id);
        }
      },
      error: (error) => {
        console.error('Error uploading photo:', error);
      }
    });
  }

  private sendMessageWithPhoto(message: SendMessage): void {
    if (!message.photoData) {
      this.hubConnection.invoke('SendMessage', message);
      return;
    }

    // Ensure content is not empty
    const messageContent = message.content?.trim() || "Photo attachment";

    // Convert the file to a base64 string instead of byte array
    const reader = new FileReader();
    reader.readAsDataURL(message.photoData);
    reader.onload = () => {
      // Get base64 data without the prefix (e.g., "data:image/jpeg;base64,")
      const base64data = (reader.result as string).split(',')[1];
      
      // Create a message DTO with the photo data
      const messageWithPhoto = {
        receiverId: message.receiverId,
        content: messageContent,
        photoData: base64data,
        photoContentType: message.photoData?.type || 'image/jpeg'
      };
      
      console.log('Sending message with photo, size:', base64data.length);
      
      // Send the message with photo data
      this.hubConnection.invoke('SendMessage', messageWithPhoto)
        .catch(err => {
          console.error('Error sending message with photo via SignalR:', err);
          // Fall back to HTTP API if SignalR fails
          this.sendPhotoViaApi(message);
        });
    };
    
    reader.onerror = (error) => {
      console.error('Error reading file:', error);
      // Fall back to HTTP API
      this.sendPhotoViaApi(message);
    };
  }

  public getPhotoUrl(message: Message): string | null {
    if (message.hasPhoto !== true || !message.photoId) {
      return null;
    }
    
    // Add timestamp to prevent caching
    return `${this.baseUrl}/message-photo/${message.photoId}?t=${new Date().getTime()}`;
  }

  public joinConversation(conversationId: number): void {
    if (this.connectionIsEstablished) {
      this.hubConnection.invoke('JoinConversation', conversationId);
    }
  }

  public leaveConversation(conversationId: number): void {
    if (this.connectionIsEstablished) {
      this.hubConnection.invoke('LeaveConversation', conversationId);
    }
  }

  public getUnreadCount(): Observable<number> {
    return this.http.get<number>(`${this.baseUrl}/unread-count`, { withCredentials: true })
      .pipe(
        tap(count => this.unreadCountSource.next(count))
      );
  }

  private refreshConversations(): void {
    this.getConversations().subscribe();
  }

  private refreshUnreadCount(): void {
    this.getUnreadCount().subscribe();
  }

  public searchUsers(searchTerm: string): Observable<UserOnline[]> {
    return this.http.get<UserOnline[]>(`${this.baseUrl}/search-users?searchTerm=${encodeURIComponent(searchTerm)}`, { withCredentials: true });
  }

  private startHeartbeat(): void {
    // Send heartbeat every 30 seconds to maintain connection
    setInterval(() => {
      if (this.connectionIsEstablished) {
        console.log('Sending heartbeat to server...');
        this.hubConnection.invoke('Heartbeat')
          .catch(err => {
            console.error('Error sending heartbeat:', err);
            // If heartbeat fails, consider connection lost and try to reconnect
            this.connectionIsEstablished = false;
            this.ensureConnection();
          });
      }
    }, 30000);
  }
} 