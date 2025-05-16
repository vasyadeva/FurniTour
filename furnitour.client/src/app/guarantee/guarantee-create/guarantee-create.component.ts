import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { GuaranteeService } from '../../services/guarantee/guarantee.service';
import { GuaranteeAddModel } from '../../models/guarantee.model';

// Інтерфейс для товарів
interface OrderItem {
  id: number;
  name: string;
  quantity: number;
  price?: number;
  category?: string;
  color?: string;
  selected: boolean;
}

@Component({
  selector: 'app-guarantee-create',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule, FormsModule],
  templateUrl: './guarantee-create.component.html',
  styleUrls: ['./guarantee-create.component.css']
})
export class GuaranteeCreateComponent implements OnInit {
  guaranteeForm!: FormGroup;
  orders: any[] = [];
  selectedOrderId: number | null = null;
  orderItems: OrderItem[] = [];
  isLoading = false;
  error = '';
  success = '';
  selectedFiles: File[] = [];
  base64Images: string[] = [];
  maxPhotos = 5;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private guaranteeService: GuaranteeService
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadUserOrders();
  }

  initForm(): void {
    this.guaranteeForm = this.fb.group({
      orderId: ['', Validators.required],
      comment: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(500)]]
    });
  }

  loadUserOrders(): void {
    this.isLoading = true;
    this.guaranteeService.getUserOrders().subscribe({
      next: (orders) => {
        if (!orders || orders.length === 0) {
          this.error = 'У вас немає замовлень, доступних для гарантійного обслуговування.';
          this.isLoading = false;
          return;
        }
        this.orders = orders;
        this.isLoading = false;
        console.log('Отримані замовлення:', this.orders);
      },
      error: (err) => {
        this.error = 'Не вдалося завантажити замовлення. Будь ласка, спробуйте ще раз.';
        this.isLoading = false;
        console.error('Помилка завантаження замовлень:', err);
      }
    });
  }

  onOrderSelect(event: any): void {
    this.isLoading = true;
    const orderId = event.target.value;
    this.selectedOrderId = Number(orderId);
    
    this.orderItems = [];
    this.error = '';
    
    if (!orderId) {
      this.isLoading = false;
      return;
    }

    this.guaranteeService.getOrderDetails(orderId).subscribe({
      next: (order) => {
        console.log('Отримано деталі замовлення:', order);
        this.processOrderItems(order);
      },
      error: (err) => {
        console.error('Помилка отримання деталей замовлення:', err);
        this.error = 'Не вдалося отримати деталі замовлення.';
        this.isLoading = false;
      }
    });
  }

  private processOrderItems(orderData: any): void {
    console.log('Обробка даних замовлення:', orderData);
    
    // For debugging - let's examine the raw order data in full
    console.log('Full order data:', JSON.stringify(orderData, null, 2));
    
    // Спочатку знайдемо масив товарів
    let items: any[] = [];
    if (orderData.orderItems && Array.isArray(orderData.orderItems)) {
      items = orderData.orderItems;
    } else {
      // Look for other possible field names
      const possibleItemFields = ['items', 'OrderItems', 'products', 'Products'];
      for (const field of possibleItemFields) {
        if (orderData[field] && Array.isArray(orderData[field]) && orderData[field].length > 0) {
          items = orderData[field];
          break;
        }
      }
    }
    
    if (items.length === 0) {
      this.error = 'У замовленні не знайдено товарів.';
      this.isLoading = false;
      return;
    }
    
    console.log('Found items:', items);
    
    // First, let's try to find the order item ID directly from the response
    // Look through the first item to see what fields it has
    if (items.length > 0) {
      console.log('First item properties:', Object.keys(items[0]));
      console.log('First item full data:', items[0]);
    }
    
    // Extract items with their actual IDs if possible
    this.orderItems = items.map((item: any) => {
      // Try to extract the actual item ID from the item
      // Prioritize fields that are likely to be order item IDs
      let itemId = null;
      
      // Look for id field directly on the item
      if (item.id !== undefined && item.id !== null) {
        itemId = item.id;
      } 
      // Look for orderItemId field (common in API responses)
      else if (item.orderItemId !== undefined && item.orderItemId !== null) {
        itemId = item.orderItemId;
      }
      // Look for Id with capital I
      else if (item.Id !== undefined && item.Id !== null) {
        itemId = item.Id;
      }
      // Look for any field with 'id' in its name
      else {
        for (const key of Object.keys(item)) {
          if (key.toLowerCase().includes('id') && item[key] !== null && item[key] !== undefined) {
            itemId = item[key];
            break;
          }
        }
      }
      
      // If we couldn't find an ID, generate one based on the index
      // But this should really be a last resort
      if (itemId === null) {
        // This should NOT happen for a correctly structured API response
        console.warn('Could not find ID for item, using fallback:', item);
        itemId = 1; // Fallback to using 1 if we can't find an ID
      }
      
      console.log(`Item ${itemId} extracted from:`, item);
      
      return {
        id: itemId,
        name: item.name || item.Name || 'Товар без назви',
        quantity: item.quantity || item.Quantity || 1,
        price: item.price || item.Price || 0,
        category: item.category || item.Category || '',
        color: item.color || item.Color || '',
        selected: false
      };
    });
    
    console.log('Final processed order items with IDs:', this.orderItems);
    this.isLoading = false;
  }

  toggleItemSelection(item: OrderItem): void {
    item.selected = !item.selected;
  }

  selectedItems(): OrderItem[] {
    return this.orderItems.filter(item => item.selected);
  }

  // Завантаження файлів
  onFileChange(event: any): void {
    const files = event.target.files;
    if (!files || files.length === 0) return;

    if (this.selectedFiles.length + files.length > this.maxPhotos) {
      this.error = `Ви можете завантажити максимум ${this.maxPhotos} фотографій.`;
      return;
    }

    for (let i = 0; i < files.length; i++) {
      const file = files[i];
      if (!file.type.match(/image\/(jpeg|jpg|png|gif)/)) {
        this.error = 'Дозволено лише зображення формату JPEG, PNG та GIF.';
        continue;
      }
      
      if (file.size > 5 * 1024 * 1024) {
        this.error = 'Розмір файлів має бути менше 5МБ.';
        continue;
      }
      
      this.selectedFiles.push(file);
      
      this.guaranteeService.convertFileToBase64(file)
        .then(base64 => {
          this.base64Images.push(base64);
        })
        .catch(err => {
          console.error('Error converting file to base64:', err);
        });
    }
  }

  removeFile(index: number): void {
    this.selectedFiles.splice(index, 1);
    this.base64Images.splice(index, 1);
  }

  async onSubmit(): Promise<void> {
    if (this.guaranteeForm.invalid || !this.selectedOrderId) {
      this.error = 'Будь ласка, заповніть всі обов\'язкові поля.';
      return;
    }
    
    const selectedItems = this.selectedItems();
    
    if (selectedItems.length === 0) {
      this.error = 'Будь ласка, виберіть принаймні один товар із вашого замовлення.';
      return;
    }
    
    this.isLoading = true;
    
    try {
      // Use the actual item IDs from the selected items
      const itemIds = selectedItems.map(item => item.id);
      
      console.log('Using actual item IDs for submission:', itemIds);
      
      const guaranteeRequest = {
        orderId: this.selectedOrderId,
        comment: this.guaranteeForm.value.comment,
        photos: this.base64Images,
        items: itemIds
      };
      
      console.log('Sending guarantee request in correct format:', JSON.stringify(guaranteeRequest));
      
      this.guaranteeService.addGuarantee(guaranteeRequest).subscribe({
        next: (response) => {
          console.log('Успіх:', response);
          this.isLoading = false;
          this.success = 'Ваш запит на гарантію успішно відправлено.';
          setTimeout(() => {
            this.router.navigate(['/guarantees']);
          }, 2000);
        },
        error: (err) => {
          this.isLoading = false;
          console.error('Помилка відправки:', err);
          
          if (err.error && typeof err.error === 'string') {
            this.error = err.error;
          } else {
            this.error = 'Не вдалося відправити запит на гарантію.';
          }
        }
      });
    } catch (error) {
      this.isLoading = false;
      this.error = 'Сталася помилка. Будь ласка, спробуйте ще раз.';
      console.error('Помилка:', error);
    }
  }
}