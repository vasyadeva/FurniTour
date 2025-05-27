import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { ProfileService } from '../../../services/profile/profile.service';
import { ProfileModel } from '../../../models/profile.model';
import { LoyaltyService } from '../../../services/loyalty/loyalty.service';
import { LoyaltyModel } from '../../../models/loyalty.model';
import { AppStatusService } from '../../../services/auth/app.status.service';
import { AuthService } from '../../../services/auth/auth.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  profile: ProfileModel = {
    username: '',
    email: '',
    phonenumber: '',
    role: ''
  };
  loyalty: LoyaltyModel | null = null;
  isOwnProfile: boolean = true;
  targetUsername: string | null = null;

  constructor(
    private profileService: ProfileService,
    private loyaltyService: LoyaltyService,
    private authService: AuthService,
    private route: ActivatedRoute,
    public status: AppStatusService
  ) { }
  ngOnInit(): void {
    // Check if there's a username parameter in the route
    this.route.paramMap.subscribe(params => {
      this.targetUsername = params.get('username');
      
      if (this.targetUsername) {
        // Viewing another user's profile
        this.isOwnProfile = false;
        this.loadUserProfile(this.targetUsername);
      } else {
        // Viewing own profile
        this.isOwnProfile = true;
        this.loadOwnProfile();
      }
    });
  }

  loadOwnProfile(): void {
    this.authService.getProfile().subscribe(
      (response: ProfileModel) => {
        console.log('Profile fetched successfully!', response);
        this.profile = response;
        this.loadLoyaltyInfo();
      },
      (error: any) => {
        console.error('Error fetching profile:', error);
      }
    );
  }
  loadUserProfile(username: string): void {
    // Use the new API to load public user profile
    this.profileService.getPublicProfile(username).subscribe({
      next: (response: ProfileModel) => {
        console.log('Public profile fetched successfully!', response);
        this.profile = response;
        // Don't load loyalty info for other users' profiles
      },
      error: (error: any) => {
        console.error('Error fetching public profile:', error);
        // If API endpoint doesn't exist yet, show a placeholder
        this.profile = {
          username: username,
          email: 'Приватна інформація',
          phonenumber: 'Приватна інформація', 
          role: 'Невідома роль'
        };
      }
    });
  }

  loadLoyaltyInfo(): void {
    if (this.status.isUser) {
      this.loyaltyService.getUserLoyalty().subscribe({
        next: (data) => {
          this.loyalty = data;
        },
        error: (error: any) => {
          console.error('Error loading loyalty info:', error);
        }
      });
    }
  }
  
  getLoyaltyBadgeClass(): string {
    if (!this.loyalty) return 'badge-standard';
    
    switch(this.loyalty.loyaltyLevel) {
      case 1: return 'badge-bronze';
      case 2: return 'badge-silver';
      case 3: return 'badge-gold';
      default: return 'badge-standard';
    }
  }
  
  getLoyaltyIcon(): string {
    if (!this.loyalty) return 'fa-user';
    
    switch(this.loyalty.loyaltyLevel) {
      case 1: return 'fa-medal text-bronze';
      case 2: return 'fa-medal text-silver';
      case 3: return 'fa-crown text-gold';
      default: return 'fa-user';
    }
  }
  
  getLoyaltyColor(): string {
    if (!this.loyalty) return '#6c757d';
    
    switch(this.loyalty.loyaltyLevel) {
      case 1: return '#cd7f32';
      case 2: return '#c0c0c0';
      case 3: return '#ffd700';
      default: return '#6c757d';
    }
  }
  
  getProgressPercentage(): number {
    if (!this.loyalty || this.loyalty.nextLevelThreshold === 0) return 0;
    
    const progress = 100 - (this.loyalty.amountToNextLevel / this.loyalty.nextLevelThreshold * 100);
    return Math.min(Math.max(progress, 0), 100); // Ensure it's between 0 and 100
  }
}
