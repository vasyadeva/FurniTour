import { Component } from '@angular/core';
import { ItemService } from '../../services/item/item.service';
import { itemGet } from '../../models/item.get.model';
import { OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CartService } from '../../services/cart/cart.service';
import { AppStatusService } from '../../services/auth/app.status.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms'; 
import { PopupService } from '../../services/popup/popup.service';

@Component({
  selector: 'app-items',
  standalone: true,
  imports: [CommonModule,RouterModule, ReactiveFormsModule, FormsModule],
  templateUrl: './items.component.html',
  styleUrl: './items.component.css'
})
export class ItemsComponent implements OnInit {
  items: itemGet[] = [];
  RecomendedItems: itemGet[] = [];
  quantity: { [key: number]: number } = {}; 
  constructor(private itemService: ItemService, private cartService : CartService, public status: AppStatusService,
    private popupService: PopupService
  ) {}

  ngOnInit(): void {
    
      this.popupService.loadingSnackBar();
    this.itemService.getAllItems().subscribe(
      (response) => {
        this.items = response;
        this.popupService.closeSnackBar();
      },
      (error) => {
        console.error('Error fetching items:', error);
        this.popupService.closeSnackBar();
        this.popupService.openSnackBar('Error fetching items');
      }
    );
    
    this.popupService.loadingSnackBar();
    this.itemService.recomended().subscribe(
      (response) => {
        console.log('Recomended items fetched successfully!', response);
        this.popupService.closeSnackBar();
        this.RecomendedItems = response;
      },
      (error) => {
        this.popupService.openSnackBar('Error fetching recomended items');
        console.error('Error fetching recomended items:', error);
      }
    );
  }

  Delete(id: number)
  {
    this.itemService.delete(id).subscribe(
      (response) => {
        console.log('Item deleted successfully!', response);
        this.popupService.openSnackBar('Item deleted successfully');
        this.items = this.items.filter(item => item.id !== id);
      },
      (error) => {
        console.error('Error deleting item:', error);
        this.popupService.openSnackBar('Error deleting item');
      }
    );
  }

  AddToCart(id: number, quantity: number)
  {
    this.cartService.AddToCart(id, quantity).subscribe(
      (response) => {
        console.log('Item added to cart successfully!', response);
        this.popupService.openSnackBar('Item added to cart successfully');

      },
      (error) => {
        console.error('Error adding item to cart:', error);
        this.popupService.openSnackBar('Error adding item to cart');
      }
    );
  }
}