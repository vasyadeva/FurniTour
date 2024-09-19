import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CreateOrderModel } from '../../models/create.order.model';
import { OrderService } from '../../services/order/order.service';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
@Component({
  selector: 'app-order',
  standalone: true,
  imports: [FormsModule, CommonModule, ReactiveFormsModule],
  templateUrl: './order.component.html',
  styleUrl: './order.component.css'
})
export class OrderComponent {
  orderForm: FormGroup;
  constructor(private fb: FormBuilder, private router: Router, private orderService: OrderService) {
    this.orderForm = this.fb.group({
      name: ['', [Validators.required]],
      address: ['', [Validators.required]],
      phone: ['', [Validators.required, Validators.pattern(/^\+?\d{10,}$/)]], // Validates phone number format
      comment: ['']
    });
  }
  submitOrder() {
    if (this.orderForm.valid) {
      const order: CreateOrderModel = this.orderForm.value;
      this.orderService.add(order).subscribe(
        response => {
          console.log('Order submitted successfully:', response);
          this.router.navigate(['/myorders']);
        },
        error => {
          console.error('Error submitting order:', error);
        }
      );
    }
  }
}
