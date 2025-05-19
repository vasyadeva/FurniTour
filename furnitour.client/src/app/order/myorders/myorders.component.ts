import { Component } from '@angular/core';
import { OrderModel } from '../../models/order.model';
import { OrderService } from '../../services/order/order.service';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { PopupService } from '../../services/popup/popup.service';

@Component({
  selector: 'app-myorders',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './myorders.component.html',
  styleUrl: './myorders.component.css'
})
export class MyordersComponent {
  orders: OrderModel[] = [];
  constructor(private orderService: OrderService,public popupService: PopupService) {
    this.popupService.loadingSnackBar();
    this.orderService.myorders().subscribe(
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
      error => {
        if (!error?.error?.isSuccess) {
            this.popupService.openSnackBar(error?.error);
        }

    }
    );
  }


}
