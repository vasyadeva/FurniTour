import { Component, OnInit } from '@angular/core';
import { CartService } from '../../services/cart/cart.service';
import { CartGet } from '../../models/cart.get.model';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { PopupService } from '../../services/popup/popup.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {
  quantity: { [key: number]: number } = {}; 
  error : string = '';
  cartItems: CartGet[] = []; 
  constructor(private cartService: CartService, private router: Router, private popupService: PopupService) {}

  ngOnInit(): void {
    this.popupService.loadingSnackBar();
    this.cartService.getCart().subscribe(
      (response) => {
        this.popupService.closeSnackBar();
        console.log('Cart:', response);
        this.cartItems = response; 
        
        // Initialize quantity object with current quantities
        this.cartItems.forEach(item => {
          this.quantity[item.id] = item.quantity;
        });
      },
      error => {
        this.popupService.closeSnackBar();
        this.error = error?.error || 'Error fetching cart';
        if (!error?.error?.isSuccess) {
            this.error = error?.error?.message || 'An unexpected error occurred. Please try again later.';
        }
      }
    );
  }
  
  removeFromCart(itemId: number): void {
    this.popupService.loadingSnackBar();
    this.cartService.removeItem(itemId).subscribe(
      () => {
        this.popupService.closeSnackBar();
        this.cartItems = this.cartItems.filter(item => item.id !== itemId);
        this.popupService.openSnackBar('Товар видалено з кошика');
      },
      error => {
        this.popupService.closeSnackBar();
        if (!error?.error?.isSuccess) {
            this.error = error?.error?.message || 'An unexpected error occurred. Please try again later.';
            this.popupService.openSnackBar(this.error);
        }
      }
    );
  }

  Order(): void {
    this.router.navigate(['/order']);
  }

  updateQuantity(itemId: number, quantity: number): void {
    if (quantity < 1) {
      quantity = 1;
      this.quantity[itemId] = 1;
    }
    
    this.popupService.loadingSnackBar();
    this.cartService.updateQuantity(itemId, quantity).subscribe(
      response => {
        this.popupService.closeSnackBar();
        // Update the item quantity in the array
        const itemIndex = this.cartItems.findIndex(item => item.id === itemId);
        if (itemIndex !== -1) {
          this.cartItems[itemIndex].quantity = quantity;
        }
        this.popupService.openSnackBar('Кількість оновлено');
      },
      error => {
        this.popupService.closeSnackBar();
        if (!error?.error?.isSuccess) {
            this.popupService.openSnackBar(error?.error || 'An unexpected error occurred. Please try again later.');
            this.error = error?.error?.message || 'An unexpected error occurred. Please try again later.';
        }
      }
    );
  }
  
  // Helper methods for order summary
  getTotalItems(): number {
    return this.cartItems.reduce((total, item) => total + item.quantity, 0);
  }
  
  getTotalPrice(): number {
    return this.cartItems.reduce((total, item) => total + (item.price * item.quantity), 0);
  }
}
