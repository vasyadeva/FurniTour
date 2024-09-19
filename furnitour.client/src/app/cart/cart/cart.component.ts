import { Component, OnInit } from '@angular/core';
import { CartService } from '../../services/cart/cart.service';
import { CartGet } from '../../models/cart.get.model';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {
  cartItems: CartGet[] = []; 
  constructor(private cartService: CartService, private router: Router) {}

  ngOnInit(): void {
    this.cartService.getCart().subscribe(
      (response) => {
        console.log('Cart:', response);
        this.cartItems = response; 
      },
      (error) => {
        console.error('Error fetching cart:', error);
      }
    );
  }
  removeFromCart(itemId: number): void {
    this.cartService.removeItem(itemId).subscribe(
      () => {
        this.cartItems = this.cartItems.filter(item => item.id !== itemId);
        console.log('Item removed from cart');
      },
      (error) => {
        console.error('Error removing item from cart:', error);
      }
    );
  }

  Order(): void {
    this.router.navigate(['/order']);
  }
}
