import { Component, OnInit } from '@angular/core';
import { ProfileManufacturerModel } from '../../../models/profile.manufacturer.model';
import { ProfileService } from '../../../services/profile/profile.service';
import { PopupService } from '../../../services/popup/popup.service';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AIReviewModel } from '../../../models/ai.review.model';

@Component({
  selector: 'app-manufacturer-profile',
  standalone: true,
  imports: [RouterModule, CommonModule],
  templateUrl: './manufacturer-profile.component.html',
  styleUrl: './manufacturer-profile.component.css'
})
export class ManufacturerProfileComponent implements OnInit {
  profile : ProfileManufacturerModel = {
    name : '',
    reviews: []
  }
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
      this.profileService.getManufacturerProfile(this.id).subscribe(
        (response) => {
          this.popupService.closeSnackBar();
          console.log('Manufacturer Profile:', response);
          this.profile = response;
        },
        (error) => {
          this.popupService.closeSnackBar();
          this.popupService.openSnackBar(error?.error || 'Error fetching manufacturer profile');
          console.error('Error fetching manufacturer profile:', error);
        }
      );
    });
    this.popupService.loadingSnackBar();
    this.profileService.getManufacturerAIReview(this.id).subscribe(
      (response) => {
        this.popupService.closeSnackBar();
        console.log('AI Review:', response);
        this.AIReview = response;
      },
      (error) => {
        this.popupService.closeSnackBar();
        this.popupService.openSnackBar(error?.error || 'Error fetching AI review');
        console.error('Error fetching AI review:', error);
      });
  }
}
