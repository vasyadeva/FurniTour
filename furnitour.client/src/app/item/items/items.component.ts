import { Component } from '@angular/core';
import { ItemService } from '../../services/item/item.service';
import { itemGet } from '../../models/item.get.model';
import { OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CartService } from '../../services/cart/cart.service';
@Component({
  selector: 'app-items',
  standalone: true,
  imports: [CommonModule,RouterModule],
  templateUrl: './items.component.html',
  styleUrl: './items.component.css'
})
export class ItemsComponent implements OnInit {
  items: itemGet[] = [];

  constructor(private itemService: ItemService, private cartService : CartService) {}

  ngOnInit(): void {
    this.itemService.getAllItems().subscribe(
      (response) => {
        this.items = response;
      },
      (error) => {
        console.error('Error fetching items:', error);
      }
    );
  }

  Delete(id: number)
  {
    this.itemService.delete(id).subscribe(
      (response) => {
        console.log('Item deleted successfully!', response);
        this.items = this.items.filter(item => item.id !== id);
      },
      (error) => {
        console.error('Error deleting item:', error);
      }
    );
  }

  AddToCart(id: number)
  {
    this.cartService.AddToCart(id).subscribe(
      (response) => {
        console.log('Item added to cart successfully!', response);
      },
      (error) => {
        console.error('Error adding item to cart:', error);
      }
    );
  }
}