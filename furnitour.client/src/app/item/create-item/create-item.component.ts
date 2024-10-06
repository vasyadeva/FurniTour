import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ItemService } from '../../services/item/item.service';
import { ReactiveFormsModule } from '@angular/forms';
import { itemSend } from '../../models/item.send.model';
import { CommonModule } from '@angular/common';
import { PopupService } from '../../services/popup/popup.service';
@Component({
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  selector: 'app-create-item',
  templateUrl: './create-item.component.html',
  styleUrls: ['./create-item.component.css']
})
export class CreateItemComponent {
  itemModel: itemSend = {
    name: '',
    description: '',
    price: 0,
    image: ''
  };
  error : string = '';
  itemForm: FormGroup;
  base64Photo: string | null = null; // Store Base64 photo

  constructor(private fb: FormBuilder, private itemService: ItemService, private popupService: PopupService) {
    this.itemForm = this.fb.group({
      name: ['', [Validators.required]],
      description: ['', [Validators.required]],
      price: [0, [Validators.required, Validators.min(0)]],
      photo: ['', [Validators.required]] // Will hold Base64 encoded image data
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
      alert('Please fill all fields and select a file.');
      return;
    }

    const base64Data = this.base64Photo.split(',')[1]; 
    this.itemModel.name = this.itemForm.get('name')?.value;
    this.itemModel.description = this.itemForm.get('description')?.value;
    this.itemModel.price = this.itemForm.get('price')?.value;
    this.itemModel.image = base64Data;
    this.itemService.create(this.itemModel).subscribe(
      response => {
        console.log('Item added successfully!', response);
        this.popupService.openSnackBar('Item added successfully!');
        this.itemForm.reset();
      },
      error => {
        if (!error?.error?.isSuccess) {
            this.popupService.openSnackBar(error?.error?.message || 'An unexpected error occurred. Please try again later.')
            this.error = error?.error?.message || 'An unexpected error occurred. Please try again later.';
        }
      }
    );
  }
}
