import { Component, OnInit } from '@angular/core';
import { ManufacturerService } from '../../services/manufacturer/manufacturer.service';
import { ManufacturerModel } from '../../models/manufacturer.model';
import { PopupService } from '../../services/popup/popup.service';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-manufacturers',
  standalone: true,
  imports: [ReactiveFormsModule, RouterModule, CommonModule],
  templateUrl: './manufacturers.component.html',
  styleUrl: './manufacturers.component.css'
})
export class ManufacturersComponent implements OnInit {
  manufacturers: ManufacturerModel[] = [];
  constructor(private manufacturerService : ManufacturerService, private popupService: PopupService) {}

  ngOnInit(): void {
    this.popupService.loadingSnackBar();
    this.manufacturerService.getAll().subscribe(data => {
      this.popupService.closeSnackBar();
      this.manufacturers = data;
    },
    error => {
      this.popupService.closeSnackBar();
      this.popupService.openSnackBar('Помилка при завантаженні виробників');
      console.log(error);
    });
  }

  deleteManufacturer(id: number) {
      this.manufacturerService.remove(id).subscribe(data => {
        this.popupService.openSnackBar('Виробник успішно видалений');
        this.ngOnInit();
      },
      error => {
        this.popupService.openSnackBar('Помилка при видаленні виробника');
        console.log(error);
      });
  }
}
