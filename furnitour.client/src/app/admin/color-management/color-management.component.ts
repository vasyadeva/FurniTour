import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService, ColorModel, ColorCreateModel, ColorUpdateModel } from '../../services/admin/admin.service';

@Component({
  selector: 'app-color-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './color-management.component.html',
  styleUrls: ['./color-management.component.css']
})
export class ColorManagementComponent implements OnInit {
  colorForm: FormGroup;
  colors: (ColorModel & { isEditing?: boolean; editName?: string })[] = [];
  filteredColors: (ColorModel & { isEditing?: boolean; editName?: string })[] = [];
  searchTerm: string = '';
  isLoading: boolean = false;
  error: string = '';
  successMessage: string = '';

  constructor(
    private fb: FormBuilder,
    private adminService: AdminService
  ) {
    this.colorForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]]
    });
  }

  ngOnInit(): void {
    this.loadColors();
  }

  loadColors(): void {
    this.isLoading = true;
    this.error = '';
    
    this.adminService.getAllColors().subscribe({
      next: (colors) => {
        this.colors = colors.map(color => ({
          ...color,
          isEditing: false,
          editName: color.name
        }));
        this.filteredColors = [...this.colors];
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading colors:', error);
        this.error = 'Не вдалося завантажити кольори';
        this.isLoading = false;
      }
    });
  }

  addColor(): void {
    if (this.colorForm.invalid) {
      this.colorForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.error = '';
    this.successMessage = '';

    const colorData: ColorCreateModel = {
      name: this.colorForm.value.name.trim()
    };

    this.adminService.createColor(colorData).subscribe({
      next: () => {
        // Reload colors to get the new one with proper ID
        this.loadColors();
        this.colorForm.reset();
        this.successMessage = 'Колір успішно додано';
        this.isLoading = false;
        
        // Clear success message after 3 seconds
        setTimeout(() => {
          this.successMessage = '';
        }, 3000);
      },
      error: (error) => {
        console.error('Error adding color:', error);
        this.error = error.error || 'Не вдалося додати колір';
        this.isLoading = false;
      }
    });
  }

  filterColors(): void {
    if (!this.searchTerm.trim()) {
      this.filteredColors = [...this.colors];
    } else {
      const searchLower = this.searchTerm.toLowerCase();
      this.filteredColors = this.colors.filter(color =>
        color.name.toLowerCase().includes(searchLower)
      );
    }
  }

  resetSearch(): void {
    this.searchTerm = '';
    this.filteredColors = [...this.colors];
  }

  startEdit(color: ColorModel & { isEditing?: boolean; editName?: string }): void {
    // Cancel any other editing colors
    this.colors.forEach(c => {
      if (c.id !== color.id) {
        c.isEditing = false;
      }
    });

    color.isEditing = true;
    color.editName = color.name;
  }

  saveColor(color: ColorModel & { isEditing?: boolean; editName?: string }): void {
    if (!color.editName?.trim()) {
      this.error = 'Назва кольору не може бути порожньою';
      return;
    }

    this.isLoading = true;
    this.error = '';

    const updateData: ColorUpdateModel = {
      id: color.id,
      name: color.editName.trim()
    };

    this.adminService.updateColor(updateData).subscribe({
      next: () => {
        color.name = updateData.name;
        color.isEditing = false;
        this.successMessage = 'Колір успішно оновлено';
        this.isLoading = false;
        this.filterColors();
        
        // Clear success message after 3 seconds
        setTimeout(() => {
          this.successMessage = '';
        }, 3000);
      },
      error: (error) => {
        console.error('Error updating color:', error);
        this.error = error.error || 'Не вдалося оновити колір';
        this.isLoading = false;
      }
    });
  }

  cancelEdit(color: ColorModel & { isEditing?: boolean; editName?: string }): void {
    color.isEditing = false;
    color.editName = color.name;
  }

  deleteColor(color: ColorModel): void {
    if (!confirm(`Ви впевнені, що хочете видалити колір "${color.name}"?`)) {
      return;
    }

    this.isLoading = true;
    this.error = '';

    // First check if color is in use
    this.adminService.isColorInUse(color.id).subscribe({
      next: (result) => {
        if (result.inUse) {
          this.error = 'Неможливо видалити колір, оскільки він використовується в товарах';
          this.isLoading = false;
          return;
        }

        // Proceed with deletion
        this.adminService.deleteColor(color.id).subscribe({
          next: () => {
            this.colors = this.colors.filter(c => c.id !== color.id);
            this.filterColors();
            this.successMessage = 'Колір успішно видалено';
            this.isLoading = false;
            
            // Clear success message after 3 seconds
            setTimeout(() => {
              this.successMessage = '';
            }, 3000);
          },
          error: (error) => {
            console.error('Error deleting color:', error);
            this.error = error.error || 'Не вдалося видалити колір';
            this.isLoading = false;
          }
        });
      },
      error: (error) => {
        console.error('Error checking color usage:', error);
        this.error = 'Не вдалося перевірити використання кольору';
        this.isLoading = false;
      }
    });
  }

  trackByColorId(index: number, color: ColorModel): number {
    return color.id;
  }
}