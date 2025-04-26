import { Component, OnInit } from '@angular/core';
import { ProfileService } from '../../../services/profile/profile.service';
import { PopupService } from '../../../services/popup/popup.service';
import { ProfileMasterModel } from '../../../models/profile.master.model';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AIReviewModel } from '../../../models/ai.review.model';
@Component({
  selector: 'app-masterprofile',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './masterprofile.component.html',
  styleUrl: './masterprofile.component.css'
})
export class MasterprofileComponent implements OnInit{
  profile : ProfileMasterModel =
  {
    username : '',
    email : '',
    phonenumber: '',
    reviews: []
  };
  profileId: string | null = null;
  AIReview: AIReviewModel = {
    review: ''
  };
  id! : string;
  constructor(private profileService: ProfileService, private popupService: PopupService,
    private route: ActivatedRoute, private router: Router,
  ) { }

  ngOnInit(): void {
    this.popupService.loadingSnackBar();

    this.route.paramMap.subscribe(params => {
      this.profileId = params.get('id');
      this.id = this.profileId!;
      this.profileService.getMasterProfile(this.id).subscribe(
        (response) => {
          this.popupService.closeSnackBar();
          console.log('Master Profile:', response);
          this.profile = response;
        },
        (error) => {
          this.popupService.closeSnackBar();
          this.popupService.openSnackBar(error?.error || 'Виникла помилка при отриманні профілю майстра');
          console.error('Виникла помилка при отриманні профілю майстра:', error);
        }
      );
      this.popupService.loadingSnackBar();
      this.profileService.getMasterAIReview(this.id).subscribe(
        (response) => {
          this.popupService.closeSnackBar();
          console.log('AI Review:', response);
          this.AIReview = response;
        },
        (error) => {
          this.popupService.closeSnackBar();
          this.popupService.openSnackBar(/*error?.error ||*/ 'Виникла помилка при отриманні ШІ відгуку');
          console.error('Виникла помилка при отриманні ШІ відгуку:', error);
        });
    });

    
  }
}
