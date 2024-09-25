import { Component, OnInit } from '@angular/core';
import { CartService } from '../../services/cart/cart.service';
import { CartGet } from '../../models/cart.get.model';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { PopupService } from '../../services/popup/popup.service';
import { FormsModule } from '@angular/forms';
@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, FormsModule],
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
    this.cartService.removeItem(itemId).subscribe(
      () => {
        this.cartItems = this.cartItems.filter(item => item.id !== itemId);
        console.log('Item removed from cart');
      },
      error => {
        if (!error?.error?.isSuccess) {
            this.error = error?.error?.message || 'An unexpected error occurred. Please try again later.';
        }

    }
    );
  }

  Order(): void {
    this.router.navigate(['/order']);
  }

  updateQuantity(itemId: number, quantity: number): void {
    this.cartService.updateQuantity(itemId, quantity).subscribe(
      response => {
        console.log('Quantity updated');
        this.popupService.openSnackBar('Quantity updated successfully');
        this.ngOnInit();
      },
      error => {
        if (!error?.error?.isSuccess) {
            this.popupService.openSnackBar(error?.error || 'An unexpected error occurred. Please try again later.');
            this.error = error?.error?.message || 'An unexpected error occurred. Please try again later.';
        }
    }
    );
  }
}
