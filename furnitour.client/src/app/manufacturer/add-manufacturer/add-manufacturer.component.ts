import { Component } from '@angular/core';
import { AddManufacturerModel } from '../../models/add.manufacturer.model';
import { ManufacturerService } from '../../services/manufacturer/manufacturer.service';
import { PopupService } from '../../services/popup/popup.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
@Component({
  selector: 'app-add-manufacturer',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './add-manufacturer.component.html',
  styleUrl: './add-manufacturer.component.css'
})
export class AddManufacturerComponent {
  manufacturerModel: AddManufacturerModel = {
    Name: ''
  };
  manufacturerForm: FormGroup;
  constructor(private fb: FormBuilder,private manufacturerService : ManufacturerService, private popupService: PopupService) {
    this.manufacturerForm = this.fb.group({
      Name: ['', [Validators.required]]
    });
  }

  onSubmit(): void {
    if (this.manufacturerForm.invalid) {
      this.popupService.openSnackBar('Please fill all fields.');
      return;
    }
    this.manufacturerModel.Name = this.manufacturerForm.get('Name')?.value;
    this.manufacturerService.add(this.manufacturerModel).subscribe(
      response => {
        this.popupService.openSnackBar('Manufacturer added successfully');
      },
      error => {
        this.popupService.openSnackBar('Error adding manufacturer');
        console.log(error);
      }
    );
  }
}
