import { Component, Input, OnInit, OnChanges, SimpleChanges, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatService, Conversation, Message, SendMessage, UserOnline } from '../../services/chat.service';
import { AuthService } from '../../services/auth/auth.service';

@Component({
  selector: 'app-chat-window',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat-window.component.html',
  styleUrls: ['./chat-window.component.css']
})
export class ChatWindowComponent implements OnInit, OnChanges, AfterViewChecked {
  @Input() conversation: Conversation | null = null;
  @Input() selectedUser: UserOnline | null = null;
  @ViewChild('scrollContainer') private scrollContainer!: ElementRef;
  
  messages: Message[] = [];
  newMessage: string = '';
  loading: boolean = false;
  error: string = '';
  userId: string = '';
  
  constructor(private chatService: ChatService, private authService: AuthService) { }

  ngOnInit(): void {
    this.chatService.messages$.subscribe(messages => {
      this.messages = messages;
    });
    
    // Get current user ID
    this.authService.credentials().subscribe(creds => {
      this.userId = creds.id;
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['conversation'] && changes['conversation'].currentValue) {
      this.loadMessages();
    }
  }

  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }

  loadMessages(): void {
    if (!this.conversation?.id) return;
    
    this.loading = true;
    this.error = '';
    
    this.chatService.getMessages(this.conversation.id).subscribe({
      next: () => {
        this.loading = false;
      },
      error: error => {
        this.loading = false;
        this.error = 'Помилка завантаження повідомлень';
        console.error('Error loading messages:', error);
      }
    });
  }

  sendMessage(): void {
    if (!this.newMessage.trim()) return;
    
    const receiverId = this.getRecipientId();
    if (!receiverId) {
      this.error = 'Не вдалося визначити отримувача';
      return;
    }
    
    const message: SendMessage = {
      receiverId: receiverId,
      content: this.newMessage.trim()
    };
    
    this.chatService.sendMessage(message);
    this.newMessage = '';
  }

  getRecipientId(): string {
    if (this.conversation) {
      return this.conversation.user1Id === this.userId 
        ? this.conversation.user2Id 
        : this.conversation.user1Id;
    }
    
    if (this.selectedUser) {
      return this.selectedUser.userId;
    }
    
    return '';
  }

  getRecipientName(): string {
    if (this.conversation) {
      return this.conversation.user1Id === this.userId 
        ? this.conversation.user2Name 
        : this.conversation.user1Name;
    }
    
    if (this.selectedUser) {
      return this.selectedUser.userName;
    }
    
    return '';
  }

  isMyMessage(message: Message): boolean {
    return message.senderId === this.userId;
  }

  formatTime(date: Date): string {
    return new Date(date).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }

  private scrollToBottom(): void {
    try {
      this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
    } catch (err) { }
  }
} 