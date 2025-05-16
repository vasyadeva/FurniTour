import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { GuaranteeService } from '../../services/guarantee/guarantee.service';
import { GuaranteeModel } from '../../models/guarantee.model';
import { AuthService } from '../../services/auth/auth.service';

@Component({
  selector: 'app-guarantee-admin',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './guarantee-admin.component.html',
  styleUrls: ['./guarantee-admin.component.css']
})
export class GuaranteeAdminComponent implements OnInit {
  guarantees: GuaranteeModel[] = [];
  filteredGuarantees: GuaranteeModel[] = [];
  sortedGuarantees: GuaranteeModel[] = [];
  loading: boolean = true;
  error: string | null = null;
  
  // Фільтри та сортування
  sortField: string = 'dateCreated';
  sortDirection: 'asc' | 'desc' = 'desc';
  statusFilter: string = '';
  userFilter: string = '';
  dateFromFilter: string = '';
  dateToFilter: string = '';
  
  // Статистика
  statusStats: { [key: string]: number } = {};
  availableStatuses: string[] = [];
  uniqueUsers: string[] = [];
  totalGuarantees: number = 0;

  constructor(
    private guaranteeService: GuaranteeService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    // Перевіряємо, чи є користувач адміністратором
    if (!this.authService.isAdmin()) {
      this.error = 'Доступ заборонено. Тільки адміністратори мають доступ до цієї сторінки.';
      this.loading = false;
      return;
    }
    
    // Завантажуємо всі гарантії
    this.loadGuarantees();
    
    // Завантажуємо список доступних статусів
    this.guaranteeService.getAvailableStatuses().subscribe({
      next: (statuses) => {
        this.availableStatuses = statuses;
      },
      error: (err) => {
        console.error('Помилка при завантаженні статусів:', err);
      }
    });
  }

  loadGuarantees(): void {
    this.loading = true;
    this.error = null;
    
    this.guaranteeService.getAllGuarantees().subscribe({
      next: (data) => {
        this.guarantees = data;
        
        // Знаходимо унікальних користувачів
        this.uniqueUsers = [...new Set(data.map(g => g.userName))];
        
        // Обраховуємо статистику
        this.calculateStatusStats();
        this.totalGuarantees = data.length;
        
        // Застосовуємо фільтрацію
        this.applyFilters();
        
        this.loading = false;
      },
      error: (err) => {
        console.error('Помилка завантаження гарантій:', err);
        this.error = 'Не вдалося завантажити список гарантій. Спробуйте пізніше.';
        this.loading = false;
      }
    });
  }

  // Підрахунок статистики по статусам
  calculateStatusStats(): void {
    this.statusStats = {};
    
    // Ініціалізуємо лічильники для всіх статусів зі значенням 0
    this.availableStatuses.forEach(status => {
      this.statusStats[status] = 0;
    });
    
    // Підраховуємо кількість гарантій за кожним статусом
    this.guarantees.forEach(guarantee => {
      if (this.statusStats[guarantee.status] !== undefined) {
        this.statusStats[guarantee.status]++;
      } else {
        // На випадок, якщо є статус, якого немає в списку доступних
        this.statusStats[guarantee.status] = 1;
      }
    });
  }

  // Застосування всіх фільтрів
  applyFilters(): void {
    // Спочатку фільтруємо всі гарантії
    this.filteredGuarantees = this.guarantees.filter(g => {
      // Фільтр за статусом
      if (this.statusFilter && g.status !== this.statusFilter) {
        return false;
      }
      
      // Фільтр за користувачем
      if (this.userFilter && g.userName !== this.userFilter) {
        return false;
      }
      
      // Фільтр за датою створення (з)
      if (this.dateFromFilter) {
        const fromDate = new Date(this.dateFromFilter);
        const createDate = new Date(g.dateCreated);
        if (createDate < fromDate) {
          return false;
        }
      }
      
      // Фільтр за датою створення (до)
      if (this.dateToFilter) {
        const toDate = new Date(this.dateToFilter);
        toDate.setHours(23, 59, 59); // Встановлюємо кінець дня
        const createDate = new Date(g.dateCreated);
        if (createDate > toDate) {
          return false;
        }
      }
      
      return true;
    });
    
    // Потім сортуємо відфільтровані гарантії
    this.applySorting();
  }

  // Функція для сортування
  applySorting(): void {
    this.sortedGuarantees = [...this.filteredGuarantees].sort((a, b) => {
      let comparison = 0;
      
      switch (this.sortField) {
        case 'id':
          comparison = a.id - b.id;
          break;
        case 'orderId':
          comparison = a.orderId - b.orderId;
          break;
        case 'userName':
          comparison = a.userName.localeCompare(b.userName);
          break;
        case 'status':
          comparison = a.status.localeCompare(b.status);
          break;
        case 'dateCreated':
          comparison = new Date(a.dateCreated).getTime() - new Date(b.dateCreated).getTime();
          break;
        case 'dateModified':
          comparison = new Date(a.dateModified).getTime() - new Date(b.dateModified).getTime();
          break;
        default:
          comparison = 0;
      }
      
      return this.sortDirection === 'asc' ? comparison : -comparison;
    });
  }

  // Функції для фільтрації
  filterByStatus(status: string): void {
    this.statusFilter = status;
    this.applyFilters();
  }
  
  filterByUser(userName: string): void {
    this.userFilter = userName;
    this.applyFilters();
  }
  
  filterByDateRange(): void {
    this.applyFilters();
  }
  
  resetFilters(): void {
    this.statusFilter = '';
    this.userFilter = '';
    this.dateFromFilter = '';
    this.dateToFilter = '';
    this.applyFilters();
  }

  // Функція для сортування
  sortGuarantees(field: string): void {
    if (this.sortField === field) {
      // Якщо поле те саме, інвертуємо напрямок сортування
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      // Якщо нове поле, встановлюємо його і напрямок за замовчуванням (за спаданням)
      this.sortField = field;
      this.sortDirection = 'desc';
    }
    
    this.applySorting();
  }
  
  // Функція для оновлення статусу гарантії
  updateStatus(guarantee: GuaranteeModel, newStatus: string): void {
    this.guaranteeService.updateGuaranteeStatus(guarantee.id, newStatus).subscribe({
      next: () => {
        guarantee.status = newStatus;
        guarantee.dateModified = new Date();
        
        // Оновлюємо статистику
        this.calculateStatusStats();
        
        // Якщо фільтр активний і гарантія більше не відповідає фільтру, оновлюємо вид
        if (this.statusFilter && this.statusFilter !== newStatus) {
          this.applyFilters();
        }
      },
      error: (err) => {
        console.error('Помилка при оновленні статусу гарантії:', err);
        alert('Не вдалося оновити статус гарантії. Спробуйте ще раз.');
      }
    });
  }
  
  // Функція для визначення класу статусу (для відображення кольорів)
  getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'очікує розгляду':
        return 'badge bg-warning';
      case 'підтверджено':
        return 'badge bg-success';
      case 'відхилено':
        return 'badge bg-danger';
      case 'в обробці':
        return 'badge bg-info';
      case 'завершено':
        return 'badge bg-primary';
      case 'скасовано':
        return 'badge bg-danger';
      case 'на розгляді':
        return 'badge bg-info';
      default:
        return 'badge bg-secondary';
    }
  }
  
  // Функція для експорту гарантій у CSV
  exportToCsv(): void {
    if (this.guarantees.length === 0) return;
    
    // Визначаємо, які дані експортувати - всі або відфільтровані
    const dataToExport = this.sortedGuarantees.length > 0 ? this.sortedGuarantees : this.guarantees;
    
    // Визначаємо заголовки колонок
    const headers = ['ID', 'ID замовлення', 'Користувач', 'Статус', 'Дата створення', 'Остання зміна', 'Коментар'];
    
    // Підготовка рядків даних
    const csvRows = [
      headers.join(','), // Заголовок
      ...dataToExport.map(guarantee => {
        return [
          guarantee.id,
          guarantee.orderId,
          guarantee.userName,
          guarantee.status,
          new Date(guarantee.dateCreated).toLocaleString(),
          new Date(guarantee.dateModified).toLocaleString(),
          `"${guarantee.comment.replace(/"/g, '""')}"` // Обробка лапок у коментарях
        ].join(',');
      })
    ];
    
    // Об'єднуємо рядки, щоб сформувати вміст CSV
    const csvContent = csvRows.join('\r\n');
    
    // Створюємо blob і посилання для завантаження
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    
    // Створюємо посилання для завантаження і клікаємо на нього
    const link = document.createElement('a');
    link.href = url;
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
    
    // Формуємо ім'я файлу, включаючи активні фільтри
    let fileName = `guarantees-export-${timestamp}`;
    if (this.statusFilter) {
      fileName += `-status-${this.statusFilter.replace(/\s+/g, '_')}`;
    }
    if (this.userFilter) {
      fileName += `-user-${this.userFilter}`;
    }
    fileName += '.csv';
    
    link.setAttribute('download', fileName);
    document.body.appendChild(link);
    link.click();
    
    // Очищаємо
    setTimeout(() => {
      document.body.removeChild(link);
      URL.revokeObjectURL(url);
    }, 100);
  }
}
