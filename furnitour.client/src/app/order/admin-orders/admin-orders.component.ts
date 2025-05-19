import { Component } from '@angular/core';
import { OrderModel } from '../../models/order.model';
import { OrderService } from '../../services/order/order.service';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { PopupService } from '../../services/popup/popup.service';
@Component({
  selector: 'app-admin-orders',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './admin-orders.component.html',
  styleUrl: './admin-orders.component.css'
})
export class AdminOrdersComponent {
  orders: OrderModel[] = [];
  constructor(private orderService: OrderService, private popupService: PopupService) {
    this.popupService.loadingSnackBar();
    this.orderService.adminorders().subscribe(
      (response) => {
        this.popupService.closeSnackBar();
        console.log('My orders:', response);
        this.orders = response;
      },
      (error) => {
        this.popupService.closeSnackBar();
        this.popupService.openSnackBar(error?.error || 'Помилка при отриманні замовлень');
        console.error('Error fetching my orders:', error);
      }
    );
  }

  updateOrder(id: number, state: number) {
    this.orderService.update(id, state).subscribe(
      (response) => {
        console.log('Order updated:', response);
        this.popupService.openSnackBar('Замовлення успішно оновлено');
      },
      (error) => {
        console.error('Error updating order:', error);
        this.popupService.openSnackBar(error?.error|| 'Сталася неочікувана помилка. Будь ласка, спробуйте пізніше.');
      }
    );
  }
}
