import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CreateOrderModel } from '../../models/create.order.model';
import { OrderService } from '../../services/order/order.service';
import { Router, RouterModule } from '@angular/router';
import { PopupService } from '../../services/popup/popup.service';
import { LoyaltyService } from '../../services/loyalty/loyalty.service';
import { CartService } from '../../services/cart/cart.service';
import { CartGet } from '../../models/cart.get.model'; // Add this import

@Component({
  selector: 'app-order',
  standalone: true,
  imports: [FormsModule, CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './order.component.html',
  styleUrl: './order.component.css'
})
export class OrderComponent implements OnInit {
  error: string = '';
  orderForm: FormGroup;
  discountPercent: number = 0;
  originalPrice: number = 0;
  discountedPrice: number = 0;
  
  constructor(
    private fb: FormBuilder,
    private router: Router,
    private orderService: OrderService,
    private popupService: PopupService,
    private loyaltyService: LoyaltyService,
    private cartService: CartService
  ) {
    this.orderForm = this.fb.group({
      name: ['', [Validators.required]],
      address: ['', [Validators.required]],
      phone: ['', [Validators.required, Validators.pattern(/^\+?\d{10,}$/)]], // Validates phone number format
      comment: ['']
    });
  }

  ngOnInit(): void {
    this.loadCartAndDiscount();
  }

  loadCartAndDiscount(): void {
    // Get cart total
    this.cartService.getCartItems().subscribe({
      next: (items: CartGet[]) => {
        this.originalPrice = items.reduce((total: number, item: CartGet) => total + (item.price * item.quantity), 0);
        
        // Get user discount
        this.loyaltyService.getUserDiscount().subscribe({
          next: (data) => {
            this.discountPercent = data.discountPercent;
            this.discountedPrice = Math.round(this.originalPrice * (1 - this.discountPercent / 100));
          },
          error: (error: any) => {
            console.error('Error loading user discount:', error);
            this.discountPercent = 0;
            this.discountedPrice = this.originalPrice;
          }
        });
      },
      error: (error: any) => {
        console.error('Error loading cart:', error);
      }
    });
  }

  submitOrder() {
    if (this.orderForm.valid) {
      this.popupService.loadingSnackBar();
      const order: CreateOrderModel = this.orderForm.value;
      this.orderService.add(order).subscribe(
        response => {
          this.popupService.closeSnackBar();
          console.log('Order submitted successfully:', response);
          this.popupService.openSnackBar('Замовлення успішно оформлено!');
          this.router.navigate(['/myorders']);
        },
        error => {
          this.popupService.closeSnackBar();
          if (!error?.error?.isSuccess) {
            this.error = error?.error?.message || 'Виникла помилка при оформленні замовлення. Спробуйте пізніше.';
            this.popupService.openSnackBar(this.error);
          }
        }
      );
    } else {
      // Mark all form controls as touched to trigger validation messages
      Object.keys(this.orderForm.controls).forEach(key => {
        const control = this.orderForm.get(key);
        control?.markAsTouched();
      });
    }
  }
}
