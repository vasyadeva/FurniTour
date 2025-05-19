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
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FurnitureReview, AddFurnitureReview } from '../../models/furniture.review.model';

@Component({
  selector: 'app-item-info',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, ReactiveFormsModule],
  templateUrl: './item-info.component.html',
  styleUrl: './item-info.component.css'
})
export class ItemInfoComponent implements OnInit {
  itemId: string | null = null;
  id!: number;
  quantity: number = 1; // Add quantity property
  
  // Reviews related properties
  reviews: FurnitureReview[] = [];
  reviewSummary: string = '';
  isLoadingSummary: boolean = false;
  currentPhotoIndex: number = 0;
  userReview: AddFurnitureReview = {
    furnitureId: 0,
    comment: '',
    rating: 5
  };
  
  item: itemGet = {
    id: 0,
    name: '',
    description: '',
    price: 0,
    category: '',
    color: '',
    image: '',
    manufacturer: '',
    master: '',
    reviews: [],
    additionalPhotos: [],
    averageRating: 0,
    reviewCount: 0
  };
  constructor(
    public itemService: ItemService, 
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
      this.userReview.furnitureId = this.id;
      
      this.itemService.details(this.id).subscribe(
        (response) => {          this.popupService.closeSnackBar();
          this.item = response;
          this.loadReviews();
          // Moved the loadReviewSummary call to be triggered after reviews are loaded
          
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
          this.popupService.openSnackBar(error?.error || 'Помилка при отриманні деталей товару');
          console.error('Error fetching item details:', error);
        }
      );
    });
  }  
  // Load reviews for the current item
  loadReviews(): void {
    this.itemService.getItemReviews(this.id).subscribe(
      (reviews) => {
        this.reviews = reviews;
        // Load summary after reviews are loaded if there are any
        if (reviews && reviews.length > 0) {
          this.loadReviewSummary();
        }
      },
      (error) => {
        console.error('Error loading reviews:', error);
      }
    );
  }
  
  // Load AI-generated review summary
  loadReviewSummary(): void {
    if (this.item.reviewCount > 0) {
      this.isLoadingSummary = true;
      this.reviewSummary = ''; // Clear existing summary
      console.log('Loading review summary for item:', this.id);
      
      this.itemService.getReviewSummary(this.id).subscribe(
        (summary) => {
          console.log('Received review summary:', summary);
          this.reviewSummary = summary;
          this.isLoadingSummary = false;
        },
        (error) => {
          console.error('Error loading review summary:', error);
          this.isLoadingSummary = false;
        }
      );
    }
  }
    // Submit a new review
  submitReview(): void {
    if (!this.userReview.comment) {
      this.popupService.openSnackBar('Будь ласка, додайте коментар до відгуку');
      return;
    }
    
    if (!this.status.isUser) {
      this.popupService.openSnackBar('Ви повинні увійти в систему, щоб залишити відгук');
      return;
    }
    
    this.itemService.addReview(this.userReview).subscribe(
      () => {
        this.popupService.openSnackBar('Відгук успішно додано');        // Reset form
        this.userReview.comment = '';
        this.userReview.rating = 5;
        // Reload reviews
        this.loadReviews();
        // No need to call loadReviewSummary here as it will be called from loadReviews
        // Reload item to get updated average rating
        this.itemService.details(this.id).subscribe(
          (response) => {
            this.item = response;
          }
        );
      },
      (error) => {
        console.error('Error adding review:', error);
        this.popupService.openSnackBar(error?.error || 'Помилка додавання відгуку');
      }
    );
  }
  
  // Photo gallery navigation
  nextPhoto(): void {
    if (this.item.additionalPhotos && this.item.additionalPhotos.length > 0) {
      this.currentPhotoIndex = (this.currentPhotoIndex + 1) % (this.item.additionalPhotos.length + 1);
    }
  }
  
  previousPhoto(): void {
    if (this.item.additionalPhotos && this.item.additionalPhotos.length > 0) {
      this.currentPhotoIndex = (this.currentPhotoIndex - 1 + this.item.additionalPhotos.length + 1) % (this.item.additionalPhotos.length + 1);
    }
  }
  
  // Get current photo URL (main photo or additional photo)
  getCurrentPhotoUrl(): string {
    if (this.currentPhotoIndex === 0) {
      return 'data:image/jpeg;base64,' + this.item.image;
    } else {
      const photo = this.item.additionalPhotos[this.currentPhotoIndex - 1];
      return this.itemService.getAdditionalImageUrl(photo.id);
    }
  }
  
  // Set the current photo index directly (for thumbnail clicks)
  setCurrentPhotoIndex(index: number): void {
    if (index >= 0 && index <= this.item.additionalPhotos.length) {
      this.currentPhotoIndex = index;
    }
  }
  
  // Updated methods for rating display with proper half-star handling
  getStars(rating: number): number[] {
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

  // Test method to verify photo count
  checkPhotoCount(): void {
    this.itemService.countAdditionalPhotos(this.id).subscribe(
      (response) => {
        this.popupService.openSnackBar(`This item has ${response.count} additional photos`);
      },
      (error) => {
        console.error('Error checking photo count:', error);
        this.popupService.openSnackBar('Помилка перевірки кількості фотографій');
      }
    );
  }
}
