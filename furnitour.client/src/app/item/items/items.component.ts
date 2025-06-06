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
import { Pipe, PipeTransform } from '@angular/core';
import {DomSanitizer} from "@angular/platform-browser";
import { AuthService } from '../../services/auth/auth.service';
import { api, copilotUrl } from '../../../environments/app.environment';

@Pipe({
  name: 'safe',
  standalone: true
})
export class SafePipe implements PipeTransform {

  constructor(private sanitizer: DomSanitizer) { }
  transform(url: any) {
    return this.sanitizer.bypassSecurityTrustResourceUrl(url);
  }

}

@Component({
  selector: 'app-items',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule, FormsModule, SidebarModule,SafePipe],
  providers: [ItemService, CartService, PopupService],
  templateUrl: './items.component.html',
  styleUrl: './items.component.css'
})

export class ItemsComponent implements OnInit {
  items: itemGet[] = [];
  RecomendedItems: itemGet[] = [];
  searchResult: itemGet[] = []; // Change from itemGet | null to itemGet[]
  quantity: { [key: number]: number } = {}; 
  searchDescription: string = '';
  
  // Make Math available in the template
  math = Math;
  
  // Updated methods for rating display with proper half-star handling
  getFullStars(rating: number): number[] {
    const fullStars = Math.floor(rating);
    return Array(fullStars).fill(0).map((_, i) => i);
  }
  
  getHalfStar(rating: number): boolean {
    // Consider a half star for decimal parts between 0.3 and 0.8
    const decimal = rating % 1;
    return decimal >= 0.3 && decimal < 0.8;
  }
  
  getEmptyStars(rating: number): number[] {
    // For decimals >= 0.8, round up to next full star
    // For decimals between 0.3 and 0.8, count as half star
    // For decimals < 0.3, round down
    let effectiveRating: number;
    const decimal = rating % 1;
    
    if (decimal >= 0.8) {
      effectiveRating = Math.ceil(rating);
    } else if (decimal >= 0.3) {
      effectiveRating = Math.floor(rating) + 0.5;
    } else {
      effectiveRating = Math.floor(rating);
    }
    
    const emptyStars = 5 - Math.ceil(effectiveRating);
    return Array(emptyStars).fill(0).map((_, i) => i);
  }
  
  // Дані для фільтрів
  categories: CategoryModel[] = [];
  colors: ColorModel[] = [];
  manufacturers: any[] = [];
  masters: any[] = [];
  UserName: string = '';
  ID : string = '';
  CopilotUrl: string = "";
  
  // Додати властивість для відстеження вибраного типу фільтра (виробник чи майстер)
  filterType: 'manufacturer' | 'master' = 'manufacturer';
  
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
  
  // Add this property to track copilot expansion state
  copilotExpanded: boolean = false;
  
  // Add this property to control welcome message visibility
  showWelcomeMessage: boolean = true;
  
  constructor(
    private itemService: ItemService, 
    private cartService: CartService, 
    public status: AppStatusService,
    private popupService: PopupService,
    private AuthService: AuthService,
  ) {}

  ngOnInit(): void {
    // Завантаження початкових даних
    this.loadAllItems();
    this.loadRecommendedItems();
    this.loadFilterOptions();
    this.AuthService.credentials().subscribe(
      (response) => {
        console.log('Profile fetched successfully!', response);
        this.UserName = response.username;
        this.ID = response.id;
        this.CopilotUrl = copilotUrl+"?&userID=" + this.ID + "&Username=" + this.UserName + "&api=" + "https://firstmintbox82.conveyor.cloud/";
  
      }
    );
    console.log(this.CopilotUrl);
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
      },
      (error) => {
        console.error('Error loading manufacturers:', error);
        this.popupService.openSnackBar('Помилка завантаження виробників');
      }
    );

    // Завантаження майстрів
    this.loadMasters();
  }
  scrollToProducts(): void {
  setTimeout(() => {
    const productsSection = document.getElementById('productsSection');
    if (productsSection) {
      productsSection.scrollIntoView({ behavior: 'smooth' });
    }
  }, 100); // Small delay to ensure filters are applied first
}
  // Метод для завантаження майстрів
  loadMasters(): void {
    this.itemService.getMasters().subscribe(
      (masters) => {
        this.masters = masters;
        this.popupService.closeSnackBar();
      },
      (error) => {
        console.error('Error loading masters:', error);
        this.popupService.openSnackBar('Помилка завантаження майстрів');
        this.popupService.closeSnackBar();
      }
    );
  }
  
  // Метод для перемикання між вибором виробника та майстра
  toggleFilterType(type: 'manufacturer' | 'master'): void {
    this.filterType = type;
    
    // Скидаємо значення, що не використовується
    if (type === 'manufacturer') {
      this.filterModel.masterID = '';
    } else {
      this.filterModel.manufacturerID = 0;
    }
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
        this.searchResult = response; // response is now properly handled as an array
        this.popupService.closeSnackBar();
      },
      (error) => {
        console.error('Error fetching item by description:', error);
        this.popupService.closeSnackBar();
        this.popupService.openSnackBar('Помилка пошуку товару за описом');
        this.searchResult = []; // Initialize as empty array on error
      }
    );
  }
  // Add this method to toggle copilot expansion
  toggleCopilot(): void {
    this.copilotExpanded = !this.copilotExpanded;
    
    // Show welcome message only if expanding and it hasn't been shown before
    if (this.copilotExpanded && this.showWelcomeMessage) {
      // Store in session storage that the message has been shown
      const hasSeenWelcome = sessionStorage.getItem('copilotWelcomeSeen');
      
      if (!hasSeenWelcome) {
        // First time in this session, show message
        this.showWelcomeMessage = true;
        sessionStorage.setItem('copilotWelcomeSeen', 'true');
        
        // Hide message after 3 seconds
        setTimeout(() => {
          this.showWelcomeMessage = false;
        }, 3000);
      } else {
        // Already seen in this session, don't show
        this.showWelcomeMessage = false;
      }
    }
  }
}
