import { Component } from '@angular/core';
import { OnInit } from '@angular/core';
import { ItemService } from '../../services/item/item.service';
import { itemGet } from '../../models/item.get.model';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { PopupService } from '../../services/popup/popup.service';
import { ClickService } from '../../services/click/click.service';
import { CartService } from '../../services/cart/cart.service';
import { AppStatusService } from '../../services/auth/app.status.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-item-info',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './item-info.component.html',
  styleUrl: './item-info.component.css'
})
export class ItemInfoComponent implements OnInit {
  itemId: string | null = null;
  id!: number;
  quantity: number = 1; // Add quantity property
  
  item: itemGet = {
    id: 0,
    name: '',
    description: '',
    price: 0,
    category: '',
    color: '',
    image: '',
    manufacturer: '',
    master: ''
  };

  constructor(
    private itemService: ItemService, 
    private route: ActivatedRoute, 
    private router: Router, 
    private popupService: PopupService,
    private clickService: ClickService,
    private cartService: CartService, // Add cart service
    public status: AppStatusService // Add status service for user role checking
  ) {}

  ngOnInit(): void {
    this.popupService.loadingSnackBar();
    this.route.paramMap.subscribe(params => {
      this.itemId = params.get('id');
      this.id = parseInt(this.itemId!);
      this.itemService.details(this.id).subscribe(
        (response) => {
          this.popupService.closeSnackBar();
          this.item = response;
          this.clickService.sendClick(this.id).subscribe(
            (response) => {
              console.log('Click sent');
            },
            (error) => {
              console.error('Error sending click:', error);
            }
          );
        },
        (error) => {
          this.popupService.closeSnackBar();
          this.popupService.openSnackBar(error?.error || 'Error fetching item details');
          console.error('Error fetching item details:', error);
        }
      );
    });
  }

  // Add method to decrement quantity with a minimum of 1
  decrementQuantity(): void {
    if (this.quantity > 1) {
      this.quantity--;
    }
  }

  // Add method to increment quantity
  incrementQuantity(): void {
    this.quantity++;
  }

  // Add to cart functionality
  AddToCart(itemId: number, quantity: number): void {
    if (!quantity || quantity < 1) {
      this.popupService.openSnackBar('Будь ласка, вкажіть кількість більше 0');
      return;
    }
    
    this.cartService.AddToCart(itemId, quantity).subscribe(
      (response) => {
        console.log('Item added to cart successfully!', response);
        this.popupService.openSnackBar('Товар успішно додано до кошика');
      },
      (error) => {
        console.error('Error adding item to cart:', error);
        this.popupService.openSnackBar('Помилка додавання товару до кошика');
      }
    );
  }
}
