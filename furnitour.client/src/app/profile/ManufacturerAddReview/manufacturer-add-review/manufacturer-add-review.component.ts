import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ManufacturerAddReviewModel } from '../../../models/manufacturer.add.review.model';
import { ProfileService } from '../../../services/profile/profile.service';
import { ActivatedRoute, Router } from '@angular/router';
import { PopupService } from '../../../services/popup/popup.service';

@Component({
  selector: 'app-manufacturer-add-review',
  standalone: true,
  imports: [FormsModule,CommonModule,ReactiveFormsModule],
  templateUrl: './manufacturer-add-review.component.html',
  styleUrl: './manufacturer-add-review.component.css'
})
export class ManufacturerAddReviewComponent implements OnInit {
  review: ManufacturerAddReviewModel = {
    name: '',
    rating: 0,
    comment: ''
  }
  profileId: string | null = null;
  id!: string;
  reviewForm: FormGroup;
  constructor(private fb: FormBuilder,private profileService: ProfileService,
    private route: ActivatedRoute, private router: Router,
    private popupService: PopupService) {
    this.reviewForm = this.fb.group({
      rating: ['',[Validators.required]],
      comment: ['']
     });
  }
  ngOnInit(): void {
    this.popupService.loadingSnackBar();
    this.route.paramMap.subscribe(params => {
      this.popupService.closeSnackBar();
      this.profileId = params.get('id');
      this.id = this.profileId!;
    });
  }

  submitReview(): void {
    if (this.reviewForm.invalid) {
      return;
    }
    this.popupService.loadingSnackBar();
    this.review  = this.reviewForm.value;
    this.review.name = this.id;
    this.profileService.addManufacturerReview(this.review).subscribe(
      (response) => {
        this.popupService.closeSnackBar();
        this.popupService.openSnackBar('Відгук успішно додано');
      },
      (error) => {
        this.popupService.closeSnackBar();
        this.popupService.openSnackBar(error?.error || 'Виникла помилка при додаванні відгуку'); 
        console.error('Виникла помилка при додаванні відгуку:', error);
      }
    );
  }
}
