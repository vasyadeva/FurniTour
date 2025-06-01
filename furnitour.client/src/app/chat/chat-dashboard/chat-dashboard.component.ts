import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription, interval } from 'rxjs';
import { ChatService, Conversation, UserOnline } from '../../services/chat.service';
import { AuthService } from '../../services/auth/auth.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatContactListComponent } from '../chat-contact-list';
import { ChatWindowComponent } from '../chat-window';

@Component({
  selector: 'app-chat-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, ChatContactListComponent, ChatWindowComponent],
  templateUrl: './chat-dashboard.component.html',
  styleUrls: ['./chat-dashboard.component.css']
})
export class ChatDashboardComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];
  conversations: Conversation[] = [];
  onlineUsers: UserOnline[] = [];
  searchResults: UserOnline[] = [];
  selectedConversation: Conversation | null = null;
  selectedUserId: string | null = null;
  searchTerm: string = '';
  view: 'conversations' | 'online' | 'search' = 'conversations';
  userId: string = '';
  isSearching: boolean = false;
  
  constructor(
    private chatService: ChatService,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    // Get user ID
    this.authService.credentials().subscribe(creds => {
      this.userId = creds.id;
      
      // Start SignalR connection
      this.startChatConnection();
    });

    // Subscribe to conversations
    this.subscriptions.push(
      this.chatService.conversations$.subscribe(conversations => {
        this.conversations = conversations;
        
        // If we have a selected conversation, refresh it
        if (this.selectedConversation) {
          const updatedConversation = conversations.find(c => c.id === this.selectedConversation!.id);
          if (updatedConversation) {
            this.selectedConversation = updatedConversation;
          }
        }
      })
    );

    // Subscribe to online users
    this.subscriptions.push(
      this.chatService.onlineUsers$.subscribe(users => {
        console.log('Online users updated in component:', users);
        // Log which users are online for debugging
        const onlineUsers = users.filter(u => u.isOnline);
        if (onlineUsers.length > 0) {
          console.log('Online users:', onlineUsers.map(u => u.userName).join(', '));
        } else {
          console.log('No users are online');
        }
        
        this.onlineUsers = users;
        
        // If we have a selected user, update its online status
        if (this.selectedUserId) {
          const selectedUser = users.find(u => u.userId === this.selectedUserId);
          if (selectedUser) {
            console.log('Selected user online status:', selectedUser.isOnline);
          }
        }
      })
    );

    // Set up periodic refresh of online users every 15 seconds
    this.subscriptions.push(
      interval(15000).subscribe(() => {
        this.refreshOnlineUsers();
      })
    );
  }

  ngOnDestroy(): void {
    this.chatService.stopConnection().subscribe();
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  loadConversations(): void {
    this.chatService.getConversations().subscribe({
      error: error => console.error('Error loading conversations:', error)
    });
  }

  refreshOnlineUsers(): void {
    console.log('Refreshing online users...');
    this.chatService.getOnlineUsers();
  }

  selectConversation(conversation: Conversation): void {
    this.selectedConversation = conversation;
    this.selectedUserId = null;
    this.chatService.joinConversation(conversation.id);
    this.chatService.markAsRead(conversation.id).subscribe();
  }
  selectUser(user: UserOnline): void {
    console.log('=== selectUser called ===');
    console.log('Selected user:', user);
    console.log('Current selectedUserId before:', this.selectedUserId);
    console.log('Current selectedConversation before:', this.selectedConversation);
    
    this.selectedUserId = user.userId;
    this.selectedConversation = null;
    
    console.log('Updated selectedUserId to:', this.selectedUserId);
    console.log('Updated selectedConversation to:', this.selectedConversation);
    
    // Try to find or create a conversation with this user
    this.chatService.getConversationWithUser(user.userId).subscribe({
      next: conversation => {
        console.log('Found existing conversation:', conversation);
        this.selectedConversation = conversation;
        this.chatService.joinConversation(conversation.id);
      },
      error: (error) => {
        // No existing conversation, just keep the user selected
        console.log('No existing conversation with user:', user.userName);
        console.log('Error details:', error);
      }
    });
  }

  search(): void {
    if (!this.searchTerm.trim()) {
      this.searchResults = [];
      return;
    }
    
    this.isSearching = true;
    this.view = 'search';
    
    // Search for both online and offline users
    console.log('Searching for users with term:', this.searchTerm);
    this.chatService.searchUsers(this.searchTerm).subscribe({
      next: (users) => {
        console.log('Search results:', users);
        this.searchResults = users;
        this.isSearching = false;
      },
      error: (error) => {
        console.error('Error searching users:', error);
        this.isSearching = false;
      }
    });
  }

  switchView(view: 'conversations' | 'online' | 'search'): void {
    this.view = view;
    if (view === 'online') {
      this.refreshOnlineUsers();
    }
  }

  getFilteredUsers(): UserOnline[] {
    // When in search view, return search results
    if (this.view === 'search') {
      return this.searchResults;
    }
    
    // When not searching, return online users (filtered if there's a search term)
    if (!this.searchTerm.trim()) {
      return this.onlineUsers;
    }
    
    return this.onlineUsers.filter(user =>
      user.userName.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  getFilteredConversations(): Conversation[] {
    if (!this.searchTerm.trim()) {
      return this.conversations;
    }
    return this.conversations.filter(conversation => {
      const otherUserName = this.getOtherUserName(conversation);
      return otherUserName.toLowerCase().includes(this.searchTerm.toLowerCase());
    });
  }

  getOtherUserName(conversation: Conversation): string {
    return conversation.user1Id === this.userId 
      ? conversation.user2Name 
      : conversation.user1Name;
  }

  getOtherUserId(conversation: Conversation): string {
    return conversation.user1Id === this.userId 
      ? conversation.user2Id 
      : conversation.user1Id;
  }
    getSelectedUser(): UserOnline | null {
    console.log('=== getSelectedUser called ===');
    console.log('selectedUserId:', this.selectedUserId);
    console.log('onlineUsers:', this.onlineUsers);
    console.log('searchResults:', this.searchResults);
    
    if (!this.selectedUserId) {
      console.log('No selectedUserId, returning null');
      return null;
    }
    
    // Check both online users and search results
    const user = [...this.onlineUsers, ...this.searchResults].find(u => u.userId === this.selectedUserId);
    console.log('Found user:', user);
    return user || null;
  }

  private startChatConnection(): void {
    console.log('Starting chat connection...');
    this.chatService.startConnection().subscribe({
      next: () => {
        console.log('Connected to chat hub');
        this.loadConversations();
        this.refreshOnlineUsers();
      },
      error: error => {
        console.error('Error connecting to chat hub:', error);
        // Retry connection after a delay
        setTimeout(() => this.startChatConnection(), 5000);
      }
    });
  }
}
 