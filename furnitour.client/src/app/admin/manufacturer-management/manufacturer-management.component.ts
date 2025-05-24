import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService, ManufacturerAdminModel, ManufacturerCreateModel, ManufacturerUpdateModel } from '../../services/admin/admin.service';

@Component({
  selector: 'app-manufacturer-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './manufacturer-management.component.html',
  styleUrls: ['./manufacturer-management.component.css']
})
export class ManufacturerManagementComponent implements OnInit {
  manufacturerForm: FormGroup;
  manufacturers: (ManufacturerAdminModel & { isEditing?: boolean; editName?: string })[] = [];
  filteredManufacturers: (ManufacturerAdminModel & { isEditing?: boolean; editName?: string })[] = [];
  searchTerm: string = '';
  isLoading: boolean = false;
  error: string = '';
  successMessage: string = '';

  constructor(
    private fb: FormBuilder,
    private adminService: AdminService
  ) {
    this.manufacturerForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]]
    });
  }

  ngOnInit(): void {
    this.loadManufacturers();
  }

  loadManufacturers(): void {
    this.isLoading = true;
    this.error = '';
    
    this.adminService.getAllManufacturers().subscribe({
      next: (manufacturers) => {
        this.manufacturers = manufacturers.map(manufacturer => ({
          ...manufacturer,
          isEditing: false,
          editName: manufacturer.name
        }));
        this.filteredManufacturers = [...this.manufacturers];
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading manufacturers:', error);
        this.error = 'Не вдалося завантажити виробників';
        this.isLoading = false;
      }
    });
  }

  addManufacturer(): void {
    if (this.manufacturerForm.invalid) {
      this.manufacturerForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.error = '';
    this.successMessage = '';

    const manufacturerData: ManufacturerCreateModel = {
      name: this.manufacturerForm.value.name.trim()
    };

    this.adminService.createManufacturer(manufacturerData).subscribe({
      next: () => {
        this.loadManufacturers();
        this.manufacturerForm.reset();
        this.successMessage = 'Виробника успішно додано';
        this.isLoading = false;
        
        setTimeout(() => {
          this.successMessage = '';
        }, 3000);
      },
      error: (error) => {
        console.error('Error adding manufacturer:', error);
        this.error = error.error || 'Не вдалося додати виробника';
        this.isLoading = false;
      }
    });
  }

  filterManufacturers(): void {
    if (!this.searchTerm.trim()) {
      this.filteredManufacturers = [...this.manufacturers];
    } else {
      const searchLower = this.searchTerm.toLowerCase();
      this.filteredManufacturers = this.manufacturers.filter(manufacturer =>
        manufacturer.name.toLowerCase().includes(searchLower)
      );
    }
  }

  resetSearch(): void {
    this.searchTerm = '';
    this.filteredManufacturers = [...this.manufacturers];
  }

  startEdit(manufacturer: ManufacturerAdminModel & { isEditing?: boolean; editName?: string }): void {
    this.manufacturers.forEach(m => {
      if (m.id !== manufacturer.id) {
        m.isEditing = false;
      }
    });

    manufacturer.isEditing = true;
    manufacturer.editName = manufacturer.name;
  }

  saveManufacturer(manufacturer: ManufacturerAdminModel & { isEditing?: boolean; editName?: string }): void {
    if (!manufacturer.editName?.trim()) {
      this.error = 'Назва виробника не може бути порожньою';
      return;
    }

    this.isLoading = true;
    this.error = '';

    const updateData: ManufacturerUpdateModel = {
      id: manufacturer.id,
      name: manufacturer.editName.trim()
    };

    this.adminService.updateManufacturer(updateData).subscribe({
      next: () => {
        manufacturer.name = updateData.name;
        manufacturer.isEditing = false;
        this.successMessage = 'Виробника успішно оновлено';
        this.isLoading = false;
        this.filterManufacturers();
        
        setTimeout(() => {
          this.successMessage = '';
        }, 3000);
      },
      error: (error) => {
        console.error('Error updating manufacturer:', error);
        this.error = error.error || 'Не вдалося оновити виробника';
        this.isLoading = false;
      }
    });
  }

  cancelEdit(manufacturer: ManufacturerAdminModel & { isEditing?: boolean; editName?: string }): void {
    manufacturer.isEditing = false;
    manufacturer.editName = manufacturer.name;
  }

  deleteManufacturer(manufacturer: ManufacturerAdminModel): void {
    if (!confirm(`Ви впевнені, що хочете видалити виробника "${manufacturer.name}"?`)) {
      return;
    }

    this.isLoading = true;
    this.error = '';

    this.adminService.isManufacturerInUse(manufacturer.id).subscribe({
      next: (result) => {
        if (result.inUse) {
          this.error = 'Неможливо видалити виробника, оскільки він використовується в товарах';
          this.isLoading = false;
          return;
        }

        this.adminService.deleteManufacturer(manufacturer.id).subscribe({
          next: () => {
            this.manufacturers = this.manufacturers.filter(m => m.id !== manufacturer.id);
            this.filterManufacturers();
            this.successMessage = 'Виробника успішно видалено';
            this.isLoading = false;
            
            setTimeout(() => {
              this.successMessage = '';
            }, 3000);
          },
          error: (error) => {
            console.error('Error deleting manufacturer:', error);
            this.error = error.error || 'Не вдалося видалити виробника';
            this.isLoading = false;
          }
        });
      },
      error: (error) => {
        console.error('Error checking manufacturer usage:', error);
        this.error = 'Не вдалося перевірити використання виробника';
        this.isLoading = false;
      }
    });
  }

  trackByManufacturerId(index: number, manufacturer: ManufacturerAdminModel): number {
    return manufacturer.id;
  }
}
