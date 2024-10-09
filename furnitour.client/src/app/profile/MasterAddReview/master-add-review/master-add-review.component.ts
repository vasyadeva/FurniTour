import { Component, OnInit } from '@angular/core';
import { ProfileService } from '../../../services/profile/profile.service';
import { ActivatedRoute, Router } from '@angular/router';
import { PopupService } from '../../../services/popup/popup.service';
import { Form, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MasterAddReviewModel } from '../../../models/master.add.review.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-master-add-review',
  standalone: true,
  imports: [FormsModule,CommonModule,ReactiveFormsModule],
  templateUrl: './master-add-review.component.html',
  styleUrl: './master-add-review.component.css'
})
export class MasterAddReviewComponent implements OnInit {
  review : MasterAddReviewModel =
  {
    username: '',
    rating: 0,
    comment: ''}
  profileId: string | null = null;
  id! : string;
  reviewFrom: FormGroup;
  constructor(private fb: FormBuilder,private profileService: ProfileService,
    private route: ActivatedRoute, private router: Router,
    private popupService: PopupService
  ) { 
    this.reviewFrom = this.fb.group({
      rating: ['',[Validators.required]],
      comment: ['']
    });
  }

  ngOnInit(): void {
    this.popupService.loadingSnackBar();

    this.route.paramMap.subscribe(params => {
      this.profileId = params.get('id');
      this.id = this.profileId!;});
  }

  submitReview(): void {
    if (this.reviewFrom.invalid) {
      return;
    }
    this.popupService.loadingSnackBar();
    this.review  = this.reviewFrom.value;
    this.review.username = this.id;
    this.profileService.addMasterReview(this.review).subscribe(
      (response) => {
        this.popupService.closeSnackBar();
        this.popupService.openSnackBar('Review added successfully');
      },
      (error) => {
        this.popupService.closeSnackBar();
        this.popupService.openSnackBar(error?.error || 'Error adding review');
        console.error('Error adding review:', error);
      }
    );
  }


}
