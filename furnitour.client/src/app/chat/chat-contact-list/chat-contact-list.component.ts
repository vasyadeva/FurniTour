import { Component, EventEmitter, Input, Output, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Conversation, UserOnline } from '../../services/chat.service';
import { AuthService } from '../../services/auth/auth.service';

@Component({
  selector: 'app-chat-contact-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat-contact-list.component.html',
  styleUrls: ['./chat-contact-list.component.css']
})
export class ChatContactListComponent implements OnInit, OnChanges {
  @Input() conversations: Conversation[] = [];
  @Input() onlineUsers: UserOnline[] = [];
  @Input() view: 'conversations' | 'online' | 'search' = 'conversations';
  @Input() searchTerm: string = '';
  @Output() conversationSelected = new EventEmitter<Conversation>();
  @Output() userSelected = new EventEmitter<UserOnline>();
  @Output() viewChange = new EventEmitter<'conversations' | 'online' | 'search'>();
  @Output() searchTermChange = new EventEmitter<string>();
  
  userId: string = '';
  onlineFilteredUsers: UserOnline[] = [];
  offlineFilteredUsers: UserOnline[] = [];
  hasOnlineUsers: boolean = false;
  hasOfflineUsers: boolean = false;

  constructor(private authService: AuthService, private router: Router) {}
  
  ngOnInit(): void {
    this.authService.credentials().subscribe(creds => {
      this.userId = creds.id;
    });
  }
  
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['onlineUsers']) {
      this.filterUsers();
    }
  }
  
  filterUsers(): void {
    this.onlineFilteredUsers = this.onlineUsers.filter(user => user.isOnline);
    this.offlineFilteredUsers = this.onlineUsers.filter(user => !user.isOnline);
    this.hasOnlineUsers = this.onlineFilteredUsers.length > 0;
    this.hasOfflineUsers = this.offlineFilteredUsers.length > 0;
  }

  selectConversation(conversation: Conversation): void {
    this.conversationSelected.emit(conversation);
  }

  selectUser(user: UserOnline): void {
    this.userSelected.emit(user);
  }

  switchView(view: 'conversations' | 'online' | 'search'): void {
    this.viewChange.emit(view);
  }

  search(): void {
    this.searchTermChange.emit(this.searchTerm);
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

  getConversationTime(conversation: Conversation): string {
    if (!conversation.lastActivity) {
      return '';
    }
    
    const date = new Date(conversation.lastActivity);
    return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }

  trackByConversationId(index: number, conversation: Conversation): number {
    return conversation.id;
  }

  trackByUserId(index: number, user: UserOnline): string {
    return user.userId;
  }
  // Navigate to user profile - now supports all user types
  navigateToProfile(user: UserOnline): void {
    this.router.navigate(['/profile/user', user.userName]);
  }

  // Navigate to profile from conversation
  navigateToConversationUserProfile(conversation: Conversation): void {
    const otherUserName = this.getOtherUserName(conversation);
    this.router.navigate(['/profile/user', otherUserName]);
  }

  // All users now have viewable profiles
  hasViewableProfile(user: UserOnline): boolean {
    return true; // All users can have their profiles viewed
  }

  // All conversation users now have viewable profiles
  conversationUserHasProfile(conversation: Conversation): boolean {
    return true; // All users can have their profiles viewed
  }
}