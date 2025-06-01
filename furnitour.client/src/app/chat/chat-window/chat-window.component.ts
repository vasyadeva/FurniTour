import { Component, Input, OnInit, OnChanges, SimpleChanges, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { Router } from '@angular/router';
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
  @ViewChild('fileInput') private fileInput!: ElementRef;
  
  messages: Message[] = [];
  newMessage: string = '';
  loading: boolean = false;
  error: string = '';
  userId: string = '';
  selectedPhoto: File | null = null;
  selectedPhotoUrl: SafeUrl | null = null;
    constructor(
    private chatService: ChatService, 
    private authService: AuthService,
    private sanitizer: DomSanitizer,
    private router: Router
  ) { }

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
    if (!this.newMessage.trim() && !this.selectedPhoto) return;
    
    const receiverId = this.getRecipientId();
    if (!receiverId) {
      this.error = 'Не вдалося визначити отримувача';
      return;
    }
    
    // Ensure we have content for the message - if only a photo is selected, use default text
    let messageContent = this.newMessage.trim();
    if (!messageContent && this.selectedPhoto) {
      messageContent = "Photo attachment";  // Default text when only photo is sent
    }
    
    console.log("Sending message with content:", messageContent);
    console.log("Has photo:", !!this.selectedPhoto);
    
    const message: SendMessage = {
      receiverId: receiverId,
      content: messageContent
    };
    
    // Add photo if selected
    if (this.selectedPhoto) {
      message.photoData = this.selectedPhoto;
      message.photoContentType = this.selectedPhoto.type;
    }
    
    this.chatService.sendMessage(message);
    this.newMessage = '';
    this.selectedPhoto = null;
    this.selectedPhotoUrl = null;
    
    // Reset file input
    if (this.fileInput) {
      this.fileInput.nativeElement.value = '';
    }
  }
  getRecipientId(): string {
    console.log('=== getRecipientId called ===');
    console.log('conversation:', this.conversation);
    console.log('selectedUser:', this.selectedUser);
    console.log('userId:', this.userId);
    
    if (this.conversation) {
      const recipientId = this.conversation.user1Id === this.userId 
        ? this.conversation.user2Id 
        : this.conversation.user1Id;
      console.log('Using conversation, recipientId:', recipientId);
      return recipientId;
    }
    
    if (this.selectedUser) {
      console.log('Using selectedUser, recipientId:', this.selectedUser.userId);
      return this.selectedUser.userId;
    }
    
    console.log('No recipient found, returning empty string');
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

  openFileSelector(): void {
    if (this.fileInput) {
      this.fileInput.nativeElement.click();
    }
  }
  
  handleFileSelect(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      
      // Check if it's an image
      if (!file.type.startsWith('image/')) {
        this.error = 'Будь ласка, виберіть зображення';
        return;
      }
      
      // Check size (max 5MB)
      if (file.size > 5 * 1024 * 1024) {
        this.error = 'Зображення занадто велике. Максимальний розмір 5MB';
        return;
      }
      
      this.selectedPhoto = file;
      // Create a safe URL for the image preview
      this.selectedPhotoUrl = this.sanitizer.bypassSecurityTrustUrl(URL.createObjectURL(file));
      this.error = '';
    }
  }
  
  removeSelectedPhoto(): void {
    if (this.selectedPhotoUrl) {
      URL.revokeObjectURL(this.selectedPhotoUrl.toString());
    }
    this.selectedPhoto = null;
    this.selectedPhotoUrl = null;
    if (this.fileInput) {
      this.fileInput.nativeElement.value = '';
    }
  }
  
  getPhotoUrl(message: Message): string | null {
    return this.chatService.getPhotoUrl(message);
  }

  isPhotoOnlyMessage(message: Message): boolean {
    return message.hasPhoto === true && message.content === 'Photo attachment';
  }  // Navigate to recipient profile - now supports all user types
  navigateToRecipientProfile(): void {
    if (this.conversation) {
      const recipientName = this.getRecipientName();
      this.router.navigate(['/profile/user', recipientName]);
    } else if (this.selectedUser) {
      this.router.navigate(['/profile/user', this.selectedUser.userName]);
    }
  }

  // All users now have viewable profiles
  recipientHasViewableProfile(): boolean {
    return true; // All users can have their profiles viewed
  }

}