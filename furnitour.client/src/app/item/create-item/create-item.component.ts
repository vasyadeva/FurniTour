import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { ItemService } from '../../services/item/item.service';
import { ReactiveFormsModule } from '@angular/forms';
import { itemSend } from '../../models/item.send.model';
import { CommonModule } from '@angular/common';
import { PopupService } from '../../services/popup/popup.service';
import { AppStatusService } from '../../services/auth/app.status.service';
import { ManufacturerModel } from '../../models/manufacturer.model';
import { ManufacturerService } from '../../services/manufacturer/manufacturer.service';
import { CategoryModel } from '../../models/category.model';
import { ColorModel } from '../../models/color.model';

@Component({
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, FormsModule],
  selector: 'app-create-item',
  templateUrl: './create-item.component.html',
  styleUrls: ['./create-item.component.css']
})
export class CreateItemComponent {
  itemModel: itemSend = {
    name: '',
    description: '',
    price: 0,
    image: '',
    categoryId: 0,
    colorId: 0,
    manufacturerId: 0
  };
  manufacturers: ManufacturerModel[] = [];
  filteredManufacturers: ManufacturerModel[] = [];
  categories: CategoryModel[] = [];
  colors: ColorModel[] = [];
  searchTerm = '';
  error: string = '';
  itemForm: FormGroup;
  base64Photo: string | null = null;

  constructor(private fb: FormBuilder, private itemService: ItemService, private popupService: PopupService,
    public status: AppStatusService, private manufacturerService: ManufacturerService
  ) {
    this.itemForm = this.fb.group({
      name: ['', [Validators.required]],
      description: ['', [Validators.required]],
      price: [0, [Validators.required, Validators.min(0)]],
      photo: ['', [Validators.required]],
      colorId: [0, [Validators.required]],
      categoryId: [0, [Validators.required]],
      manufacturerId: [null, status.isAdmin ? [Validators.required] : []]
    });

    this.manufacturerService.getAll().subscribe(
      response => {
        this.manufacturers = response;
        this.filterManufacturers();
      },
      error => {
        this.popupService.openSnackBar(error?.error?.message || 'An unexpected error occurred. Please try again later.');
      }
    );

    this.itemService.getCategories().subscribe(
      response => {
        this.categories = response;
      },
      error => {
        this.popupService.openSnackBar(error?.error?.message || 'An unexpected error occurred. Please try again later.');
      }
    );

    this.itemService.getColors().subscribe(
      response => {
        this.colors = response;
      },
      error => {
        this.popupService.openSnackBar(error?.error?.message || 'An unexpected error occurred. Please try again later.');
      }
    );

    this.filterManufacturers();
  }

  filterManufacturers() {
    const term = this.searchTerm.toLowerCase();
    this.filteredManufacturers = this.manufacturers
      .filter(manufacturer => manufacturer.name.toLowerCase().includes(term))
      .slice(0, 5);
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = () => {
        this.base64Photo = reader.result as string;
        this.itemForm.patchValue({ photo: this.base64Photo });
      };
      reader.readAsDataURL(file);
    }
  }

  onSubmit(): void {
    if (this.itemForm.invalid || !this.base64Photo) {
      alert('Please fill all fields and select a file.');
      return;
    }

    const base64Data = this.base64Photo.split(',')[1];
    this.itemModel.name = this.itemForm.get('name')?.value;
    this.itemModel.description = this.itemForm.get('description')?.value;
    this.itemModel.price = this.itemForm.get('price')?.value;
    this.itemModel.categoryId = this.itemForm.get('categoryId')?.value;
    this.itemModel.colorId = this.itemForm.get('colorId')?.value;
    this.itemModel.image = base64Data;
    this.itemModel.manufacturerId = this.itemForm.get('manufacturerId')?.value;
    this.itemService.create(this.itemModel).subscribe(
      response => {
        console.log('Item added successfully!', response);
        this.popupService.openSnackBar('Item added successfully!');
        this.itemForm.reset();
      },
      error => {
        if (!error?.error?.isSuccess) {
          this.popupService.openSnackBar(error?.error?.message || 'An unexpected error occurred. Please try again later.');
          this.error = error?.error?.message || 'An unexpected error occurred. Please try again later.';
        }
      }
    );
  }
}
