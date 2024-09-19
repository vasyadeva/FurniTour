import { Component } from '@angular/core';
import { OrderModel } from '../../models/order.model';
import { OrderService } from '../../services/order/order.service';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-myorders',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './myorders.component.html',
  styleUrl: './myorders.component.css'
})
export class MyordersComponent {
  orders: OrderModel[] = [];
  constructor(private orderService: OrderService) {
    this.orderService.myorders().subscribe(
      (response) => {
        console.log('My orders:', response);
        this.orders = response;
      },
      (error) => {
        console.error('Error fetching my orders:', error);
      }
    );
  }

  updateOrder(id: number, state: number) {
    this.orderService.update(id, state).subscribe(
      (response) => {
        console.log('Order updated:', response);
      },
      (error) => {
        console.error('Error updating order:', error);
      }
    );
  }
}
