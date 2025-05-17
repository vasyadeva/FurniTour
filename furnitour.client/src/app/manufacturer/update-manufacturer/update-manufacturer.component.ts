import { Component, model, OnInit } from '@angular/core';
import { ManufacturerModel } from '../../models/manufacturer.model';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ManufacturerService } from '../../services/manufacturer/manufacturer.service';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { PopupService } from '../../services/popup/popup.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-update-manufacturer',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './update-manufacturer.component.html',
  styleUrl: './update-manufacturer.component.css'
})
export class UpdateManufacturerComponent implements OnInit {
  manufacturerId : string | null = null;
  id! : number;
  model : ManufacturerModel ={
    id: 0,
    name: ''
  }
  form: FormGroup;
  constructor(private fb: FormBuilder, private manufacturerService: ManufacturerService,
    private route: ActivatedRoute, private router: Router, private popupService: PopupService
  ) { 
    this.form = this.fb.group({
      name: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.manufacturerId = params.get('id');
      this.id = parseInt(this.manufacturerId!);
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      return;
    }
    this.model.name = this.form.get('name')?.value;
    this.model.id = this.id;
    this.popupService.loadingSnackBar();
    this.manufacturerService.update(this.model).subscribe(
      (response) => {
        this.popupService.closeSnackBar();
        this.popupService.openSnackBar('Manufacturer updated successfully');
        this.router.navigate(['/manufacturer']);
      },
      (error) => {
        this.popupService.closeSnackBar();
        this.popupService.openSnackBar(error?.error || 'Error updating manufacturer');
        console.error('Error updating manufacturer:', error);
      }
    );
  }

}
