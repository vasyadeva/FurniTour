import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { ItemService } from '../../services/item/item.service';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { PopupService } from '../../services/popup/popup.service';
import { itemUpdate } from '../../models/item.update.model';
import { AppStatusService } from '../../services/auth/app.status.service';
import { ManufacturerModel } from '../../models/manufacturer.model';
import { ManufacturerService } from '../../services/manufacturer/manufacturer.service';
import { CategoryModel } from '../../models/category.model';
import { ColorModel } from '../../models/color.model';
import { FurnitureAdditionalPhoto } from '../../models/furniture.review.model';

@Component({
  selector: 'app-item-edit',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, FormsModule, RouterModule],
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
    categoryId: 0,
    colorId: 0,
    manufacturerId: 0,
    additionalPhotos: [],
    photoDescriptions: [],
    photoIdsToRemove: []
  };

  manufacturers: ManufacturerModel[] = [];
  filteredManufacturers: ManufacturerModel[] = [];
  categories: CategoryModel[] = [];
  colors: ColorModel[] = [];
  searchTerm = '';

  error: string = '';
  itemForm: FormGroup;
  base64Photo: string | null = null;
  loading: boolean = true;
  
  // Additional photos
  additionalPhotos: string[] = [];
  photoDescriptions: string[] = [];
  existingAdditionalPhotos: FurnitureAdditionalPhoto[] = [];
  photoIdsToRemove: number[] = [];
  constructor(private fb: FormBuilder, public itemService: ItemService, private route: ActivatedRoute, private router: Router,
    private popupService: PopupService, public status: AppStatusService, private manufacturerService: ManufacturerService
  ) {
    // Initialize form with empty values - we'll populate it in ngOnInit
    if (status.isAdmin) {
      this.itemForm = this.fb.group({
        name: ['', [Validators.required]],
        description: ['', [Validators.required]],
        price: [0, [Validators.required, Validators.min(0)]],
        colorId: [0, [Validators.required]],
        categoryId: [0, [Validators.required]],
        photo: [''],  // Remove required validator since we'll use existing image
        manufacturerId: [null, [Validators.required]]
      });
    } else {
      this.itemForm = this.fb.group({
        name: ['', [Validators.required]],
        description: ['', [Validators.required]],
        price: [0, [Validators.required, Validators.min(0)]],
        colorId: [0, [Validators.required]],
        categoryId: [0, [Validators.required]],
        photo: [''],  // Remove required validator
        manufacturerId: [null]
      });
    }

    // Fetch required data for dropdowns
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

  ngOnInit(): void {
    this.popupService.loadingSnackBar();
    this.route.paramMap.subscribe(params => {
      this.itemId = params.get('id');
      if (this.itemId) {
        this.id = parseInt(this.itemId);
        
        // Fetch item details and populate the form
        this.itemService.details(this.id).subscribe(
          (itemDetails) => {
            this.loading = false;
            this.popupService.closeSnackBar();
            
            // Get category ID and color ID from the loaded data
            const categoryId = this.getCategoryIdByName(itemDetails.category);
            const colorId = this.getColorIdByName(itemDetails.color);
            let manufacturerId = null;
            
            // Find manufacturer ID if exists
            if (itemDetails.manufacturer && this.manufacturers.length > 0) {
              const manufacturer = this.manufacturers.find(m => m.name === itemDetails.manufacturer);
              if (manufacturer) {
                manufacturerId = manufacturer.id;
              }
            }
            
            // Update form with item details
            this.itemForm.patchValue({
              name: itemDetails.name,
              description: itemDetails.description,
              price: itemDetails.price,
              categoryId: categoryId,
              colorId: colorId,
              manufacturerId: manufacturerId
            });
            
            // Set base64 photo for preview
            if (itemDetails.image) {
              this.base64Photo = 'data:image/jpeg;base64,' + itemDetails.image;
            }
              // Set item model
            this.itemModel = {
              id: itemDetails.id,
              name: itemDetails.name,
              description: itemDetails.description,
              price: itemDetails.price,
              image: itemDetails.image,
              categoryId: categoryId,
              colorId: colorId,
              manufacturerId: manufacturerId || 0,
              additionalPhotos: [],
              photoDescriptions: [],
              photoIdsToRemove: []
            };
            
            // Store existing additional photos
            if (itemDetails.additionalPhotos && itemDetails.additionalPhotos.length > 0) {
              this.existingAdditionalPhotos = itemDetails.additionalPhotos;
            }
          },
          (error) => {
            this.loading = false;
            this.popupService.closeSnackBar();
            this.error = 'Failed to load item details. Please try again.';
            this.popupService.openSnackBar('Error loading item details: ' + (error.error || 'Unknown error'));
          }
        );
      } else {
        this.loading = false;
        this.popupService.closeSnackBar();
        this.error = 'No item ID provided';
      }
    });
  }

  // Helper methods to get IDs from names
  getCategoryIdByName(categoryName: string): number {
    const category = this.categories.find(c => c.name === categoryName);
    return category ? category.id : 0;
  }

  getColorIdByName(colorName: string): number {
    const color = this.colors.find(c => c.name === colorName);
    return color ? color.id : 0;
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

  // Add an additional photo
  onAdditionalPhotoSelected(event: any): void {
    if (event.target.files && event.target.files[0]) {
      const file = event.target.files[0];
      const reader = new FileReader();
      reader.onload = (e: any) => {
        const base64String = e.target.result.split(',')[1];
        this.additionalPhotos.push(base64String);
        this.photoDescriptions.push('');
      };
      reader.readAsDataURL(file);
    }
  }
  
  // Remove a pending additional photo
  removeAdditionalPhoto(index: number): void {
    this.additionalPhotos.splice(index, 1);
    this.photoDescriptions.splice(index, 1);
  }
  
  // Mark an existing photo for removal
  removeExistingPhoto(photoId: number): void {
    this.photoIdsToRemove.push(photoId);
    // Remove from the displayed list
    this.existingAdditionalPhotos = this.existingAdditionalPhotos.filter(p => p.id !== photoId);
  }
  
  // Update photo description
  updatePhotoDescription(index: number, description: string): void {
    this.photoDescriptions[index] = description;
  }
  onSubmit(): void {
    if (this.itemForm.invalid) {
      this.popupService.openSnackBar('Please fill all required fields.');
      return;
    }

    // Extract base64 data for a new uploaded image or keep existing one
    let base64Data: string;
    if (this.base64Photo?.includes('data:image')) {
      base64Data = this.base64Photo.split(',')[1];
    } else {
      base64Data = this.itemModel.image;
    }
    
    // Update the item model with form values
    this.itemModel.id = this.id;
    this.itemModel.name = this.itemForm.get('name')?.value;
    this.itemModel.description = this.itemForm.get('description')?.value;
    this.itemModel.price = this.itemForm.get('price')?.value;
    this.itemModel.categoryId = this.itemForm.get('categoryId')?.value;
    this.itemModel.colorId = this.itemForm.get('colorId')?.value;
    this.itemModel.image = base64Data;
    this.itemModel.manufacturerId = this.itemForm.get('manufacturerId')?.value;
    
    // Set additional photos data
    this.itemModel.additionalPhotos = this.additionalPhotos;
    this.itemModel.photoDescriptions = this.photoDescriptions;
    this.itemModel.photoIdsToRemove = this.photoIdsToRemove;
    
    this.itemService.update(this.itemModel).subscribe(
      response => {
        this.popupService.openSnackBar('Item updated successfully!');
        this.router.navigate(['/items']);
      },
      error => {
        if (!error?.error?.isSuccess) {
          this.popupService.openSnackBar(error?.error || 'An unexpected error occurred. Please try again later.');
        }
      }
    );
  }
}
