import { Component, OnInit } from '@angular/core';
import { ProfileManufacturerModel } from '../../../models/profile.manufacturer.model';
import { ProfileService } from '../../../services/profile/profile.service';
import { PopupService } from '../../../services/popup/popup.service';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

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
  }
}
