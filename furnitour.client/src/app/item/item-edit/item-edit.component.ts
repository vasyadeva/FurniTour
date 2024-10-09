import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { ItemService } from '../../services/item/item.service';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { PopupService } from '../../services/popup/popup.service';
import { itemUpdate } from '../../models/item.update.model';
import { AppStatusService } from '../../services/auth/app.status.service';
import { ManufacturerModel } from '../../models/manufacturer.model';
import { ManufacturerService } from '../../services/manufacturer/manufacturer.service';
@Component({
  selector: 'app-item-edit',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, FormsModule],
  templateUrl: './item-edit.component.html',
  styleUrl: './item-edit.component.css'
})
export class ItemEditComponent implements OnInit {
  itemId: string | null = null;
  id!: number;
  itemModel: itemUpdate = {
    id: 0,
    name: '',
    description: '',
    price: 0,
    image: '',
    manufacturerId: 0
  };

  manufacturers: ManufacturerModel[] = [];
  filteredManufacturers : ManufacturerModel[]= [];
  searchTerm = '';

  error : string = '';
  itemForm: FormGroup;
  base64Photo: string | null = null; 

  constructor(private fb: FormBuilder, private itemService: ItemService,private route: ActivatedRoute, private router: Router,
    private popupService: PopupService, public status: AppStatusService,private manufacturerService: ManufacturerService
  ) {
    if (status.isAdmin)
      {
        this.itemForm = this.fb.group({
          name: ['', [Validators.required]],
          description: ['', [Validators.required]],
          price: [0, [Validators.required, Validators.min(0)]],
          photo: ['', [Validators.required]],
          manufacturerId: [null, [Validators.required]] 
        });
      }
      else
      {
        this.itemForm = this.fb.group({
          name: ['', [Validators.required]],
          description: ['', [Validators.required]],
          price: [0, [Validators.required, Validators.min(0)]],
          photo: ['', [Validators.required]],
          manufacturerId: [null] 
        });
      }

      this.manufacturerService.getAll().subscribe(
        response =>{
          this.manufacturers = response;
          this.filterManufacturers();
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

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.itemId = params.get('id');
      this.id = parseInt(this.itemId!);
    });
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
      this.popupService.openSnackBar('Please fill all fields and select a file.');
      return;
    }

    const base64Data = this.base64Photo.split(',')[1]; 
    this.itemModel.id = this.id;
    this.itemModel.name = this.itemForm.get('name')?.value;
    this.itemModel.description = this.itemForm.get('description')?.value;
    this.itemModel.price = this.itemForm.get('price')?.value;
    this.itemModel.image = base64Data;
    this.itemModel.manufacturerId = this.itemForm.get('manufacturerId')?.value; 
    this.itemService.update(this.itemModel).subscribe(
      response => {
        this.popupService.openSnackBar('Item updated successfully!');

        this.itemForm.reset();
      },
      error => {
        if (!error?.error?.isSuccess) {
            this.popupService.openSnackBar(error?.error || 'An unexpected error occurred. Please try again later.');
        }
      }
    );
  }
}
