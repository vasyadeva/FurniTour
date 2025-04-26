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
import { SidebarModule } from '@coreui/angular';
import { ItemFilterModel } from '../../models/item.filter.model';
import { CategoryModel } from '../../models/category.model';
import { ColorModel } from '../../models/color.model';

@Component({
  selector: 'app-items',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule, FormsModule, SidebarModule],
  providers: [ItemService, CartService, AppStatusService, PopupService],
  templateUrl: './items.component.html',
  styleUrl: './items.component.css'
})
export class ItemsComponent implements OnInit {
  items: itemGet[] = [];
  RecomendedItems: itemGet[] = [];
  searchResult: itemGet | null = null;
  quantity: { [key: number]: number } = {}; 
  searchDescription: string = '';
  
  // Дані для фільтрів
  categories: CategoryModel[] = [];
  colors: ColorModel[] = [];
  manufacturers: any[] = [];
  
  // Модель фільтрації
  filterModel: ItemFilterModel = {
    colorID: 0,
    categoryID: 0,
    manufacturerID: 0,
    masterID: '',
    minPrice: 0,
    maxPrice: 0,
    searchString: ''
  };
  
  constructor(
    private itemService: ItemService, 
    private cartService: CartService, 
    public status: AppStatusService,
    private popupService: PopupService
  ) {}

  ngOnInit(): void {
    // Завантаження початкових даних
    this.loadAllItems();
    this.loadRecommendedItems();
    this.loadFilterOptions();
  }
  
  // Завантаження даних для фільтрів
  loadFilterOptions(): void {
    this.popupService.loadingSnackBar();
    
    // Завантаження категорій
    this.itemService.getCategories().subscribe(
      (categories) => {
        this.categories = categories;
      },
      (error) => {
        console.error('Error loading categories:', error);
        this.popupService.openSnackBar('Помилка завантаження категорій');
      }
    );
    
    // Завантаження кольорів
    this.itemService.getColors().subscribe(
      (colors) => {
        this.colors = colors;
      },
      (error) => {
        console.error('Error loading colors:', error);
        this.popupService.openSnackBar('Помилка завантаження кольорів');
      }
    );
    
    // Завантаження виробників
    this.itemService.getManufacturers().subscribe(
      (manufacturers) => {
        this.manufacturers = manufacturers;
        this.popupService.closeSnackBar();
      },
      (error) => {
        console.error('Error loading manufacturers:', error);
        this.popupService.openSnackBar('Помилка завантаження виробників');
        this.popupService.closeSnackBar();
      }
    );
  }
  
  // Завантаження всіх товарів
  loadAllItems(): void {
    this.popupService.loadingSnackBar();
    this.itemService.getAllItems().subscribe(
      (response) => {
        this.items = response;
        this.popupService.closeSnackBar();
      },
      (error) => {
        console.error('Error fetching items:', error);
        this.popupService.closeSnackBar();
        this.popupService.openSnackBar('Помилка завантаження товарів');
      }
    );
  }
  
  // Завантаження рекомендованих товарів
  loadRecommendedItems(): void {
    this.popupService.loadingSnackBar();
    this.itemService.recomended().subscribe(
      (response) => {
        console.log('Recomended items fetched successfully!', response);
        this.popupService.closeSnackBar();
        this.RecomendedItems = response;
      },
      (error) => {
        this.popupService.openSnackBar('Помилка завантаження рекомендованих товарів');
        console.error('Error fetching recomended items:', error);
        this.popupService.closeSnackBar();
      }
    );
  }
  
  // Застосування фільтрів
  applyFilters(): void {
    this.popupService.loadingSnackBar();
    this.itemService.getFilteredItems(this.filterModel).subscribe(
      (response) => {
        this.items = response;
        this.popupService.closeSnackBar();
      },
      (error) => {
        console.error('Error applying filters:', error);
        this.popupService.closeSnackBar();
        this.popupService.openSnackBar('Помилка застосування фільтрів');
      }
    );
  }
  
  // Скидання фільтрів
  resetFilters(): void {
    this.filterModel = {
      colorID: 0,
      categoryID: 0,
      manufacturerID: 0,
      masterID: '',
      minPrice: 0,
      maxPrice: 0,
      searchString: ''
    };
    this.loadAllItems();
  }

  Delete(id: number) {
    this.itemService.delete(id).subscribe(
      (response) => {
        console.log('Item deleted successfully!', response);
        this.popupService.openSnackBar('Товар успішно видалено');
        this.items = this.items.filter(item => item.id !== id);
      },
      (error) => {
        console.error('Error deleting item:', error);
        this.popupService.openSnackBar('Помилка видалення товару');
      }
    );
  }

  AddToCart(id: number, quantity: number) {
    if (!quantity || quantity < 1) {
      this.popupService.openSnackBar('Будь ласка, вкажіть кількість більше 0');
      return;
    }
    
    this.cartService.AddToCart(id, quantity).subscribe(
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

  searchItem() {
    if (this.searchDescription.trim() === '') {
      this.popupService.openSnackBar('Будь ласка, введіть опис для пошуку');
      return;
    }

    this.popupService.loadingSnackBar();
    this.itemService.getItemByDescription(this.searchDescription).subscribe(
      (response) => {
        this.searchResult = response;
        this.popupService.closeSnackBar();
      },
      (error) => {
        console.error('Error fetching item by description:', error);
        this.popupService.closeSnackBar();
        this.popupService.openSnackBar('Помилка пошуку товару за описом');
      }
    );
  }
}