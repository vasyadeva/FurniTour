import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { IndividualOrderService } from '../../services/individual-order/individual-order.service';
import { IndividualOrderModel } from '../../models/individual.order.model';
import { AuthService } from '../../services/auth/auth.service';

// Інтерфейс для статусів в процесі замовлення
interface StatusItem {
  id: number;
  name: string;
  isActive?: boolean;
}

@Component({  selector: 'app-individual-order-details',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './individual-order-details.component.html',
  styleUrl: './individual-order-details.component.css'
})
export class IndividualOrderDetailsComponent implements OnInit {
  order: IndividualOrderModel | null = null;
  loading: boolean = true;
  orderId: number = 0;
  
  isMaster: boolean = false;
  isAdmin: boolean = false;
  
  // Визначення статусів та їх порядку
  private statusOrder: { [key: string]: number } = {
    'нове індивідуальне замовлення': 1,
    'підтверджено': 2,
    'у виробництві': 3,
    'в дорозі': 4,
    'доставлено': 5,
    'доставка підтверджена користувачем': 6,
    'скасовано користувачем': 99,
    'скасовано майстром': 99
  };
  
  priceForm: FormGroup;
  notesForm: FormGroup;
  errorMessage: string = '';
  successMessage: string = '';

  constructor(
    private route: ActivatedRoute,
    private individualOrderService: IndividualOrderService,
    private authService: AuthService,
    private fb: FormBuilder
  ) {
    this.priceForm = this.fb.group({
      estimatedPrice: ['', [Validators.required, Validators.min(1)]],
      finalPrice: ['', [Validators.min(1)]]
    });
    
    this.notesForm = this.fb.group({
      notes: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.orderId = +this.route.snapshot.paramMap.get('id')!;
    
    this.checkRole();
    this.loadOrderDetails();
  }
  checkRole(): void {
    this.authService.getUserRole().subscribe({
      next: (role: string) => {
        this.isMaster = role === 'Master';
        this.isAdmin = role === 'Administrator';
      },
      error: () => {
        this.errorMessage = 'Не вдалося перевірити роль користувача';
      }
    });
  }  loadOrderDetails(): void {
    this.loading = true;
    // Clear messages when refreshing data
    this.successMessage = '';
    this.errorMessage = '';
    
    this.individualOrderService.getIndividualOrderDetails(this.orderId).subscribe({
      next: (order) => {
        this.order = order;
        console.log('Завантажено замовлення:', order); // Додано для дебагу
        
        // Заповнюємо форму поточними даними
        this.priceForm.patchValue({
          estimatedPrice: order.estimatedPrice,
          finalPrice: order.finalPrice
        });
        
        if (order.masterNotes) {
          this.notesForm.patchValue({
            notes: order.masterNotes
          });
        }
        
        // Додаємо перевірку чи є URL зображення
        if (order.photo) {
          console.log('URL зображення:', order.photo);
        } else {
          console.log('Зображення відсутнє для цього замовлення');
        }
        
        this.loading = false;
      },error: (error) => {
        this.errorMessage = 'Не вдалося завантажити деталі замовлення';
        this.loading = false;
        console.error('Помилка при завантаженні деталей замовлення:', error);
      }
    });
  }

  getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'нове індивідуальне замовлення':
        return 'badge bg-info';
      case 'підтверджено':
        return 'badge bg-primary';
      case 'у виробництві':
        return 'badge bg-warning';
      case 'в дорозі':
        return 'badge bg-secondary';
      case 'доставлено':
        return 'badge bg-success';
      case 'скасовано замовником':
      case 'скасовано майстром':
        return 'badge bg-danger';
      default:
        return 'badge bg-light text-dark';
    }
  }  changeStatus(statusId: number): void {
    console.log('Змінюємо статус: ID замовлення =', this.orderId, 'Новий статус ID =', statusId);
    this.individualOrderService.changeIndividualOrderStatus(this.orderId, statusId).subscribe({
      next: () => {
        this.successMessage = 'Статус замовлення змінено';
        this.errorMessage = '';
        this.loadOrderDetails(); // Оновлюємо дані
      },
      error: (error) => {
        this.errorMessage = this.getStatusErrorMessage(error);
        this.successMessage = '';
        console.error('Помилка при зміні статусу замовлення:', error);
        console.error('Деталі запиту:', { 
          orderId: this.orderId, 
          statusId: statusId, 
          поточний_статус: this.order?.status 
        }); // Додано для дебагу
      }
    });
  }
  onPriceFormSubmit(): void {
    if (this.priceForm.valid) {
      this.errorMessage = '';
      this.successMessage = '';
      const estimatedPrice = this.priceForm.get('estimatedPrice')?.value;
      const finalPrice = this.priceForm.get('finalPrice')?.value;
      let updatesCompleted = 0;
      let hasErrors = false;

      const updateCompleteCheck = () => {
        updatesCompleted++;
        const totalUpdates = (estimatedPrice ? 1 : 0) + (finalPrice ? 1 : 0);
        if (updatesCompleted === totalUpdates && !hasErrors) {
          this.successMessage = 'Ціни успішно оновлено';
          // Оновлюємо дані замовлення
          setTimeout(() => this.loadOrderDetails(), 500);
        }
      };

      // Встановлення орієнтовної ціни
      if (estimatedPrice) {
        this.individualOrderService.setEstimatedPrice(this.orderId, estimatedPrice).subscribe({
          next: () => {
            updateCompleteCheck();
          },
          error: (error) => {
            this.errorMessage = 'Не вдалося встановити орієнтовну ціну: ' + (error.error || '');
            hasErrors = true;
            console.error('Помилка при встановленні орієнтовної ціни:', error);
          }
        });
      }

      // Встановлення фінальної ціни
      if (finalPrice) {
        this.individualOrderService.setFinalPrice(this.orderId, finalPrice).subscribe({
          next: () => {
            updateCompleteCheck();
          },
          error: (error) => {
            if (!this.errorMessage) {
              this.errorMessage = 'Не вдалося встановити фінальну ціну: ' + (error.error || '');
            } else {
              this.errorMessage += ' Також не вдалося встановити фінальну ціну.';
            }
            hasErrors = true;
            console.error('Помилка при встановленні фінальної ціни:', error);
          }
        });
      }

      // Якщо немає ні estimatedPrice, ні finalPrice, показуємо повідомлення
      if (!estimatedPrice && !finalPrice) {
        this.errorMessage = 'Будь ласка, вкажіть хоча б одну ціну для оновлення';
      }
    }
  }
  onNotesFormSubmit(): void {
    if (this.notesForm.valid) {
      const notes = this.notesForm.get('notes')?.value;
      
      this.errorMessage = '';
      this.successMessage = '';
      
      this.individualOrderService.addMasterNotes(this.orderId, notes).subscribe({
        next: () => {
          this.successMessage = 'Коментар майстра додано';
          this.loadOrderDetails(); // Оновлюємо дані
        },
        error: (error) => {
          this.errorMessage = 'Не вдалося додати коментар: ' + (error.error || '');
          console.error('Помилка при додаванні коментаря:', error);
        }
      });
    }
  }  // Визначення доступних статусів для поточного замовлення
  getAvailableStatusChanges(): StatusItem[] {
    if (!this.order) return [];
    
    const currentStatus = this.order.status.toLowerCase();
    const result: StatusItem[] = [];

    // Для користувача
    if (!this.isMaster && !this.isAdmin) {
      // Користувач може скасувати замовлення до виробництва
      if (['нове індивідуальне замовлення', 'підтверджено'].includes(currentStatus)) {
        result.push({ id: 2, name: 'Скасувати замовлення' });
      }
      
      // Підтвердити доставку
      if (currentStatus === 'доставлено') {
        result.push({ id: 8, name: 'Підтвердити отримання' });
      }
      
      // Додаємо інформаційний блок для користувача
      result.push({ id: -4, name: 'Статус вашого замовлення:' });
      
      // Показуємо всі можливі статуси для інформації
      result.push({ id: -5, name: 'Нове індивідуальне замовлення', isActive: currentStatus === 'нове індивідуальне замовлення' });
      result.push({ id: -5, name: 'Підтверджено', isActive: currentStatus === 'підтверджено' });
      result.push({ id: -5, name: 'У виробництві', isActive: currentStatus === 'у виробництві' });
      result.push({ id: -5, name: 'В дорозі', isActive: currentStatus === 'в дорозі' });
      result.push({ id: -5, name: 'Доставлено', isActive: currentStatus === 'доставлено' });
      result.push({ id: -5, name: 'Доставка підтверджена користувачем', isActive: currentStatus === 'доставка підтверджена користувачем' });
      
      // Добавляем информационное сообщение для скансованого замовлення
      if (['скасовано користувачем', 'скасовано майстром'].includes(currentStatus)) {
        result.push({ id: -2, name: 'Замовлення скасоване і не може бути змінене' });
      }
    }
      
    // Для майстра або адміністратора
    if (this.isMaster || this.isAdmin) {
      // Перевіряємо, чи замовлення не скасоване та не завершене
      const isCancelled = ['скасовано користувачем', 'скасовано майстром'].includes(currentStatus);
      const isCompleted = currentStatus === 'доставка підтверджена користувачем';
      
      if (!isCancelled && !isCompleted) {
        // ВАЖЛИВО: На сервері є обмеження, що ми можемо переходити лише до наступного статусу (+1)
        // або до статусу "Скасовано майстром" (id=3)
        
        // Додаємо інформацію про доступні дії
        result.push({ id: -3, name: 'Доступні дії:' });
        
        // Завжди показуємо кнопку скасування майстром для доступних станів
        if (!['у виробництві', 'в дорозі', 'доставлено', 'доставка підтверджена користувачем'].includes(currentStatus)) {
          result.push({ id: 3, name: 'Скасувати замовлення' });
        }
        
        // Показуємо активні кнопки для переходу на наступний статус з чіткими назвами
        switch (currentStatus) {
          case 'нове індивідуальне замовлення':
            result.push({ id: 4, name: 'Підтвердити замовлення' });
            break;
          case 'підтверджено':
            result.push({ id: 5, name: 'Перевести у виробництво' });
            break;
          case 'у виробництві':
            result.push({ id: 6, name: 'Відправити замовлення' });
            break;
          case 'в дорозі':
            result.push({ id: 7, name: 'Позначити як доставлене' });
            break;
          case 'доставлено':
            // Тільки користувач може підтвердити отримання
            result.push({ id: -2, name: 'Очікування підтвердження отримання від клієнта' });
            break;
        }
        
        // Додаємо пояснення про послідовність зміни статусів
        result.push({ id: -4, name: 'Послідовність зміни статусів:' });
        
        // Показуємо всі можливі статуси з індикацією поточного
        result.push({ id: -5, name: 'Нове індивідуальне замовлення', isActive: currentStatus === 'нове індивідуальне замовлення' });
        result.push({ id: -5, name: 'Підтверджено', isActive: currentStatus === 'підтверджено' });
        result.push({ id: -5, name: 'У виробництві', isActive: currentStatus === 'у виробництві' });
        result.push({ id: -5, name: 'В дорозі', isActive: currentStatus === 'в дорозі' });
        result.push({ id: -5, name: 'Доставлено', isActive: currentStatus === 'доставлено' });
        result.push({ id: -5, name: 'Доставка підтверджена користувачем', isActive: currentStatus === 'доставка підтверджена користувачем' });
      }
      
      // Додаємо інформаційне повідомлення про поточний статус
      result.push({ id: -1, name: `Поточний статус: ${this.order.status}` });
      
      // Додаємо повідомлення, якщо замовлення скасоване або завершене
      if (isCancelled) {
        result.push({ id: -2, name: 'Замовлення скасоване і не може бути змінене' });
      } else if (isCompleted) {
        result.push({ id: -2, name: 'Замовлення завершене' });
      }
    }
    
    return result;
  }
  
  // Перевіряє, чи статус вже пройдено
  isStatusCompleted(statusName: string): boolean {
    if (!this.order) return false;
    
    const currentStatusOrder = this.statusOrder[this.order.status.toLowerCase()];
    const checkedStatusOrder = this.statusOrder[statusName.toLowerCase()];
    
    // Якщо статус не скасовано, перевіряємо чи він пройдений
    if (currentStatusOrder < 90) {
      return checkedStatusOrder < currentStatusOrder;
    }
    
    // Для скасованих замовлень
    return false;
  }
  
  // Перевіряє, чи статус ще попереду
  isStatusFuture(statusName: string): boolean {
    if (!this.order) return false;
    
    // Для скасованих замовлень всі статуси крім скасування є "майбутніми"
    if (this.order.status.toLowerCase().includes('скасовано')) {
      return !statusName.toLowerCase().includes('скасовано');
    }
    
    const currentStatusOrder = this.statusOrder[this.order.status.toLowerCase()];
    const checkedStatusOrder = this.statusOrder[statusName.toLowerCase()];
    
    return checkedStatusOrder > currentStatusOrder;
  }
  
  // Повертає іконку для статусу
  getStatusIcon(status: StatusItem): string {
    if (status.isActive) {
      return 'bi bi-arrow-right-circle-fill me-1';
    } else if (this.isStatusCompleted(status.name)) {
      return 'bi bi-check-circle-fill me-1';
    } else {
      return 'bi bi-circle me-1';
    }
  }
  
  // Допоміжний метод для виведення зрозумілого повідомлення про помилку зміни статусу
  private getStatusErrorMessage(error: any): string {
    if (error && error.error) {
      // Якщо повідомлення "Неможливо пропустити проміжні статуси"
      if (error.error.includes('пропустити проміжні статуси')) {
        return 'Неможливо пропустити проміжні статуси. Статуси замовлення мають змінюватися послідовно.';
      }
      
      // Інші помилки зі статусом
      return error.error;
    }
    
    // Загальна помилка
    return 'Виникла помилка при зміні статусу замовлення';
  }

  // Метод для відображення зображення у повному розмірі
  openFullsizeImage(imageUrl: string): void {
    window.open(imageUrl, '_blank');
  }
}
