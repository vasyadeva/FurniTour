import { Component, OnInit } from '@angular/core';
import { ManufacturerService } from '../../services/manufacturer/manufacturer.service';
import { ManufacturerModel } from '../../models/manufacturer.model';
import { PopupService } from '../../services/popup/popup.service';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-manufacturers',
  standalone: true,
  imports: [ReactiveFormsModule, RouterModule, CommonModule, FormsModule],
  templateUrl: './manufacturers.component.html',
  styleUrl: './manufacturers.component.css'
})
export class ManufacturersComponent implements OnInit {
  manufacturers: ManufacturerModel[] = [];
  allManufacturers: ManufacturerModel[] = []; // Store all manufacturers for filtering
  searchTerm: string = '';
  
  constructor(private manufacturerService : ManufacturerService, private popupService: PopupService) {}
  ngOnInit(): void {
    this.popupService.loadingSnackBar();
    this.manufacturerService.getAll().subscribe(data => {
      this.popupService.closeSnackBar();
      this.manufacturers = data;
      this.allManufacturers = data; // Save all manufacturers for search functionality
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
  
  searchManufacturers(): void {
    if (!this.searchTerm.trim()) {
      this.manufacturers = this.allManufacturers;
      return;
    }
    
    const term = this.searchTerm.toLowerCase();
    this.manufacturers = this.allManufacturers.filter(
      manufacturer => manufacturer.name.toLowerCase().includes(term) || 
                      manufacturer.id.toString().includes(term)
    );
  }
}
