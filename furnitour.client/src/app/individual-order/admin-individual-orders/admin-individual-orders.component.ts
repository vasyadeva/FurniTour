import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { IndividualOrderService } from '../../services/individual-order/individual-order.service';
import { IndividualOrderModel } from '../../models/individual.order.model';

@Component({  selector: 'app-admin-individual-orders',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './admin-individual-orders.component.html',
  styleUrl: './admin-individual-orders.component.css'
})
export class AdminIndividualOrdersComponent implements OnInit {
  orders: IndividualOrderModel[] = [];
  filteredOrders: IndividualOrderModel[] = [];
  loading: boolean = true;
  filterForm: FormGroup;
  errorMessage: string = '';

  constructor(
    private individualOrderService: IndividualOrderService,
    private fb: FormBuilder
  ) {
    this.filterForm = this.fb.group({
      status: [''],
      priceCategory: [''],
      searchTerm: ['']
    });
  }

  ngOnInit(): void {
    this.loadOrders();

    // Підписка на зміни форми фільтрації
    this.filterForm.valueChanges.subscribe(() => {
      this.applyFilters();
    });
  }
  loadOrders(): void {
    this.loading = true;
    this.individualOrderService.getAllIndividualOrders().subscribe({
      next: (data) => {
        this.orders = data;
        this.filteredOrders = [...this.orders];
        this.loading = false;
      },
      error: (error) => {
        this.errorMessage = 'Не вдалося завантажити замовлення';
        this.loading = false;
        console.error('Помилка при завантаженні замовлень:', error);
      }
    });
  }

  applyFilters(): void {
    const filterValue = this.filterForm.value;
    
    this.filteredOrders = this.orders.filter(order => {
      // Фільтр за статусом
      if (filterValue.status && order.status !== filterValue.status) {
        return false;
      }
      
      // Фільтр за ціновою категорією
      if (filterValue.priceCategory && order.priceCategory !== filterValue.priceCategory) {
        return false;
      }
      
      // Пошук за текстом
      if (filterValue.searchTerm) {
        const searchTerm = filterValue.searchTerm.toLowerCase();
        return order.name.toLowerCase().includes(searchTerm) ||
               order.description.toLowerCase().includes(searchTerm) ||
               order.userName.toLowerCase().includes(searchTerm);
      }
      
      return true;
    });
  }

  resetFilters(): void {
    this.filterForm.reset({
      status: '',
      priceCategory: '',
      searchTerm: ''
    });
  }

  getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'нове індивідуальне замовлення':
        return 'badge bg-info';
      case 'підтверджено':
        return 'badge bg-primary';
      case 'у виробництві':
        return 'badge bg-warning';
      case 'в дорозі':
        return 'badge bg-secondary';
      case 'доставлено':
        return 'badge bg-success';
      case 'скасовано замовником':
      case 'скасовано майстром':
        return 'badge bg-danger';
      default:
        return 'badge bg-light text-dark';
    }
  }

  // Унікальні статуси замовлень для фільтра
  getUniqueStatuses(): string[] {
    const statuses = this.orders.map(order => order.status);
    return [...new Set(statuses)];
  }

  // Унікальні цінові категорії для фільтра
  getUniquePriceCategories(): string[] {
    const categories = this.orders.map(order => order.priceCategory);
    return [...new Set(categories)];
  }
}
