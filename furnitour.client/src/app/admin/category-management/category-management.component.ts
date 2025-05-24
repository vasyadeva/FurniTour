import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService, CategoryModel, CategoryCreateModel, CategoryUpdateModel } from '../../services/admin/admin.service';

@Component({
  selector: 'app-category-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './category-management.component.html',
  styleUrls: ['./category-management.component.css']
})
export class CategoryManagementComponent implements OnInit {
  categoryForm: FormGroup;
  categories: (CategoryModel & { isEditing?: boolean; editName?: string })[] = [];
  filteredCategories: (CategoryModel & { isEditing?: boolean; editName?: string })[] = [];
  searchTerm: string = '';
  isLoading: boolean = false;
  error: string = '';
  successMessage: string = '';

  constructor(
    private fb: FormBuilder,
    private adminService: AdminService
  ) {
    this.categoryForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]]
    });
  }

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.isLoading = true;
    this.error = '';
    
    this.adminService.getAllCategories().subscribe({
      next: (categories) => {
        this.categories = categories.map(category => ({
          ...category,
          isEditing: false,
          editName: category.name
        }));
        this.filteredCategories = [...this.categories];
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading categories:', error);
        this.error = 'Не вдалося завантажити категорії';
        this.isLoading = false;
      }
    });
  }

  addCategory(): void {
    if (this.categoryForm.invalid) {
      this.categoryForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.error = '';
    this.successMessage = '';

    const categoryData: CategoryCreateModel = {
      name: this.categoryForm.value.name.trim()
    };

    this.adminService.createCategory(categoryData).subscribe({
      next: () => {
        this.loadCategories();
        this.categoryForm.reset();
        this.successMessage = 'Категорію успішно додано';
        this.isLoading = false;
        
        setTimeout(() => {
          this.successMessage = '';
        }, 3000);
      },
      error: (error) => {
        console.error('Error adding category:', error);
        this.error = error.error || 'Не вдалося додати категорію';
        this.isLoading = false;
      }
    });
  }

  filterCategories(): void {
    if (!this.searchTerm.trim()) {
      this.filteredCategories = [...this.categories];
    } else {
      const searchLower = this.searchTerm.toLowerCase();
      this.filteredCategories = this.categories.filter(category =>
        category.name.toLowerCase().includes(searchLower)
      );
    }
  }

  resetSearch(): void {
    this.searchTerm = '';
    this.filteredCategories = [...this.categories];
  }

  startEdit(category: CategoryModel & { isEditing?: boolean; editName?: string }): void {
    this.categories.forEach(c => {
      if (c.id !== category.id) {
        c.isEditing = false;
      }
    });

    category.isEditing = true;
    category.editName = category.name;
  }

  saveCategory(category: CategoryModel & { isEditing?: boolean; editName?: string }): void {
    if (!category.editName?.trim()) {
      this.error = 'Назва категорії не може бути порожньою';
      return;
    }

    this.isLoading = true;
    this.error = '';

    const updateData: CategoryUpdateModel = {
      id: category.id,
      name: category.editName.trim()
    };

    this.adminService.updateCategory(updateData).subscribe({
      next: () => {
        category.name = updateData.name;
        category.isEditing = false;
        this.successMessage = 'Категорію успішно оновлено';
        this.isLoading = false;
        this.filterCategories();
        
        setTimeout(() => {
          this.successMessage = '';
        }, 3000);
      },
      error: (error) => {
        console.error('Error updating category:', error);
        this.error = error.error || 'Не вдалося оновити категорію';
        this.isLoading = false;
      }
    });
  }

  cancelEdit(category: CategoryModel & { isEditing?: boolean; editName?: string }): void {
    category.isEditing = false;
    category.editName = category.name;
  }

  deleteCategory(category: CategoryModel): void {
    if (!confirm(`Ви впевнені, що хочете видалити категорію "${category.name}"?`)) {
      return;
    }

    this.isLoading = true;
    this.error = '';

    this.adminService.isCategoryInUse(category.id).subscribe({
      next: (result) => {
        if (result.inUse) {
          this.error = 'Неможливо видалити категорію, оскільки вона використовується в товарах';
          this.isLoading = false;
          return;
        }

        this.adminService.deleteCategory(category.id).subscribe({
          next: () => {
            this.categories = this.categories.filter(c => c.id !== category.id);
            this.filterCategories();
            this.successMessage = 'Категорію успішно видалено';
            this.isLoading = false;
            
            setTimeout(() => {
              this.successMessage = '';
            }, 3000);
          },
          error: (error) => {
            console.error('Error deleting category:', error);
            this.error = error.error || 'Не вдалося видалити категорію';
            this.isLoading = false;
          }
        });
      },
      error: (error) => {
        console.error('Error checking category usage:', error);
        this.error = 'Не вдалося перевірити використання категорії';
        this.isLoading = false;
      }
    });
  }

  trackByCategoryId(index: number, category: CategoryModel): number {
    return category.id;
  }
}
