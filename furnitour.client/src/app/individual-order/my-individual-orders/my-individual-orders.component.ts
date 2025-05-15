import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { IndividualOrderService } from '../../services/individual-order/individual-order.service';
import { IndividualOrderModel } from '../../models/individual.order.model';

@Component({  selector: 'app-my-individual-orders',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './my-individual-orders.component.html',
  styleUrl: './my-individual-orders.component.css'
})
export class MyIndividualOrdersComponent implements OnInit {
  orders: IndividualOrderModel[] = [];
  loading: boolean = true;
  errorMessage: string = '';

  constructor(
    private individualOrderService: IndividualOrderService
  ) { }

  ngOnInit(): void {
    this.loadOrders();
  }
  loadOrders(): void {
    this.loading = true;
    this.individualOrderService.getMyIndividualOrders().subscribe({
      next: (data) => {
        this.orders = data;
        this.loading = false;
      },
      error: (error) => {
        this.errorMessage = 'Не вдалося завантажити замовлення';
        this.loading = false;
        console.error('Помилка при завантаженні замовлень:', error);
      }
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
}
