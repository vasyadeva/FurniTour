import { Component, OnInit } from '@angular/core';
import { CartService } from '../../services/cart/cart.service';
import { CartGet } from '../../models/cart.get.model';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { PopupService } from '../../services/popup/popup.service';
import { FormsModule } from '@angular/forms';
import { LoyaltyService } from '../../services/loyalty/loyalty.service';
import { AppStatusService } from '../../services/auth/app.status.service'; // Add this import

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
  discountPercent: number = 0;
  
  constructor(
    private cartService: CartService, 
    private router: Router, 
    private popupService: PopupService,
    private loyaltyService: LoyaltyService,
    public status: AppStatusService // Add this property
  ) {}

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
        this.error = error?.error || 'Помилка при отриманні кошику';
        if (!error?.error?.isSuccess) {
            this.error = error?.error?.message || 'Сталася неочікувана помилка. Будь ласка, спробуйте пізніше.';
        }
      }
    );

    this.loadUserDiscount();
  }

  loadUserDiscount(): void {
    if (this.status.isUser) {
      this.loyaltyService.getUserDiscount().subscribe({
        next: (data) => {
          this.discountPercent = data.discountPercent;
        },
        error: (error) => {
          console.error('Error loading user discount:', error);
          this.discountPercent = 0;
        }
      });
    }
  }
  
  // Method to get price with discount applied
  getDiscountedPrice(): number {
    const totalPrice = this.getTotalPrice();
    if (this.discountPercent > 0) {
      return Math.round(totalPrice * (1 - this.discountPercent / 100));
    }
    return totalPrice;
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
            this.error = error?.error?.message || 'Помилка при видаленні товару з кошика';
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
            this.popupService.openSnackBar(error?.error || 'Сталася неочікувана помилка. Будь ласка, спробуйте пізніше.');
            this.error = error?.error?.message || 'Сталася неочікувана помилка. Будь ласка, спробуйте пізніше.';
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
