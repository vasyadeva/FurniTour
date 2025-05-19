import { Component, OnInit } from '@angular/core';
import { ManufacturerService } from '../../services/manufacturer/manufacturer.service';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { PopupService } from '../../services/popup/popup.service';
import { ManufacturerModel } from '../../models/manufacturer.model';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-info-manufacturer',
  standalone: true,
  imports: [CommonModule,RouterModule],
  templateUrl: './info-manufacturer.component.html',
  styleUrl: './info-manufacturer.component.css'
})
export class InfoManufacturerComponent implements OnInit {
  manufacturerId : string | null = null;
  id! : number;
  model : ManufacturerModel ={
    id: 0,
    name: ''
  }
  constructor(private manufacturerService: ManufacturerService, private route: ActivatedRoute, private popupService: PopupService) {}
  ngOnInit(): void {
    this.popupService.loadingSnackBar();
    this.route.paramMap.subscribe(params => {
      this.manufacturerId = params.get('id');
      this.id = parseInt(this.manufacturerId!);
      this.manufacturerService.get(this.id).subscribe(
        (response) => {
          this.popupService.closeSnackBar();
          this.model = response;
        },
        (error) => {
          this.popupService.closeSnackBar();
          this.popupService.openSnackBar(error?.error || 'Помилка при отриманні деталей виробника');
          console.error('Error fetching manufacturer details:', error);
        }
      );
    });
  }
}
