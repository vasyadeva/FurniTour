import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { IndividualOrderService } from '../../services/individual-order/individual-order.service';
import { PriceCategoryModel } from '../../models/price.category.model';
import { Router, RouterModule } from '@angular/router';
import { MasterModel } from '../../models/master.model';

@Component({  selector: 'app-create-individual-order',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './create-individual-order.component.html',
  styleUrl: './create-individual-order.component.css'
})
export class CreateIndividualOrderComponent implements OnInit {
  orderForm: FormGroup;
  priceCategories: PriceCategoryModel[] = [];
  masters: MasterModel[] = [];
  selectedFile: File | null = null;
  loading: boolean = false;
  errorMessage: string = '';
  successMessage: string = '';

  constructor(
    private fb: FormBuilder,
    private individualOrderService: IndividualOrderService,
    private router: Router
  ) {    this.orderForm = this.fb.group({
      name: ['', [Validators.required]],
      address: ['', [Validators.required]],
      phone: ['', [Validators.required, Validators.pattern('[0-9]{10}')]],
      description: ['', [Validators.required, Validators.minLength(10)]],
      priceCategoryId: ['', [Validators.required]],
      masterId: ['', [Validators.required]]
    });
  }  ngOnInit(): void {
    this.loadPriceCategories();
    this.loadMasters();
  }
  loadMasters(): void {
    this.individualOrderService.getMasters().subscribe({
      next: (masters) => {
        this.masters = masters;
      },
      error: (error) => {
        this.errorMessage = 'Не вдалося завантажити список майстрів';
        console.error('Помилка при завантаженні списку майстрів:', error);
      }
    });
  }

  loadPriceCategories(): void {
    this.individualOrderService.getPriceCategories().subscribe({
      next: (categories) => {
        this.priceCategories = categories;
      },
      error: (error) => {
        this.errorMessage = 'Не вдалося завантажити цінові категорії';
        console.error('Помилка при завантаженні цінових категорій:', error);
      }
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length) {
      this.selectedFile = input.files[0];
    }
  }
  onSubmit(): void {
    if (this.orderForm.valid) {
      this.loading = true;
      this.errorMessage = '';
      this.successMessage = '';
      
      const orderData = {
        ...this.orderForm.value,
        photo: this.selectedFile
      };

      this.individualOrderService.createIndividualOrder(orderData).subscribe({
        next: () => {
          this.successMessage = 'Ваше індивідуальне замовлення успішно створено';
          this.loading = false;
          setTimeout(() => {
            this.router.navigate(['/individual-orders']);
          }, 2000);
        },
        error: (error) => {
          this.errorMessage = 'Не вдалося створити замовлення: ' + (error.error || 'Виникла помилка');
          this.loading = false;
          console.error('Помилка при створенні замовлення:', error);
        }
      });
    } else {
      this.orderForm.markAllAsTouched();
      this.errorMessage = 'Будь ласка, заповніть всі обов\'язкові поля';
    }
  }
}
