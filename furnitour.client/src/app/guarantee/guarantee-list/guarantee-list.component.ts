import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { GuaranteeService } from '../../services/guarantee/guarantee.service';
import { GuaranteeModel } from '../../models/guarantee.model';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth/auth.service';

@Component({
  selector: 'app-guarantee-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './guarantee-list.component.html',
  styleUrls: ['./guarantee-list.component.css']
})
export class GuaranteeListComponent implements OnInit {
  statusStats: { [key: string]: number } = {};
  
  sortField: string = 'dateCreated';
  sortDirection: 'asc' | 'desc' = 'desc';
  statusFilter: string = '';
  
  guarantees: GuaranteeModel[] = [];
  filteredGuarantees: GuaranteeModel[] = [];
  sortedGuarantees: GuaranteeModel[] = [];
  loading: boolean = true;
  error: string | null = null;
  isAdmin: boolean = false;
  availableStatuses: string[] = [];

  constructor(
    private guaranteeService: GuaranteeService,
    private authService: AuthService
  ) { }
  ngOnInit(): void {
    this.loadGuarantees();
    
    // Якщо це адмін, завантажуємо список доступних статусів
    if (this.authService.isAdmin()) {
      this.guaranteeService.getAvailableStatuses().subscribe({
        next: (statuses) => {
          this.availableStatuses = statuses;
        },
        error: (err) => {
          console.error('Помилка при завантаженні статусів:', err);
        }
      });
    }
  }  loadGuarantees(): void {
    this.loading = true;
    this.error = null;
    
    // Check if we're viewing all guarantees or just the user's guarantees
    const isAdmin = this.authService.isAdmin();
    this.isAdmin = isAdmin;
    
    const observable = isAdmin 
      ? this.guaranteeService.getAllGuarantees() 
      : this.guaranteeService.getMyGuarantees();
      
    observable.subscribe({
      next: (data) => {
        this.guarantees = data;
        
        if (isAdmin) {
          this.calculateStatusStats();
        }
        
        this.applyFiltering(); // Спочатку фільтруємо
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading guarantees:', err);
        this.error = 'Не вдалося завантажити гарантії. Спробуйте пізніше.';
        this.loading = false;
      }
    });
  }

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
  
  filterByStatus(status: string): void {
    this.statusFilter = status;
    this.applyFiltering();
  }
  
  // Підрахунок статистики по статусам
  private calculateStatusStats(): void {
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
  
  private applyFiltering(): void {
    if (!this.statusFilter) {
      this.filteredGuarantees = [...this.guarantees];
    } else {
      this.filteredGuarantees = this.guarantees.filter(g => g.status === this.statusFilter);
    }
    
    this.applySorting();
  }
    private applySorting(): void {
    this.sortedGuarantees = [...this.filteredGuarantees].sort((a, b) => {
      let comparison = 0;
      
      switch (this.sortField) {
        case 'id':
          comparison = a.id - b.id;
          break;        case 'orderId':
          // Handle individual orders properly
          if (a.isIndividualOrder && b.isIndividualOrder) {
            // Both are individual orders, compare individual order IDs
            const aIndOrderId = a.individualOrderId || 0;
            const bIndOrderId = b.individualOrderId || 0;
            comparison = aIndOrderId - bIndOrderId;
          } else if (a.isIndividualOrder && !b.isIndividualOrder) {
            // a is individual, b is regular - individual orders first
            comparison = -1;
          } else if (!a.isIndividualOrder && b.isIndividualOrder) {
            // a is regular, b is individual - individual orders first
            comparison = 1;
          } else {
            // Both are regular orders
            const aOrderId = a.orderId || 0;
            const bOrderId = b.orderId || 0;
            comparison = aOrderId - bOrderId;
          }
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

  exportToCsv(): void {
    if (!this.isAdmin || this.guarantees.length === 0) return;
    
    // Визначити, які дані експортувати - всі або відфільтровані
    const dataToExport = this.statusFilter ? this.sortedGuarantees : this.guarantees;
      // Визначаємо заголовки колонок
    const headers = ['ID', 'Тип замовлення', 'ID замовлення', 'Користувач', 'Статус', 'Дата створення', 'Остання зміна', 'Коментар'];
    
    // Підготовка рядків даних
    const csvRows = [
      headers.join(','), // Заголовок
      ...dataToExport.map(guarantee => {
        return [
          guarantee.id,
          guarantee.isIndividualOrder ? 'Індивідуальне' : 'Звичайне',
          guarantee.isIndividualOrder ? guarantee.individualOrderId : guarantee.orderId,
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
    const fileName = `guarantees-export-${timestamp}.csv`;
    link.setAttribute('download', fileName);
    document.body.appendChild(link);
    link.click();
    
    // Очищаємо
    setTimeout(() => {
      document.body.removeChild(link);
      URL.revokeObjectURL(url);
    }, 100);
  }
    updateStatus(guarantee: GuaranteeModel, newStatus: string): void {
    if (!this.isAdmin) return;
    
    this.guaranteeService.updateGuaranteeStatus(guarantee.id, newStatus).subscribe({
      next: () => {
        guarantee.status = newStatus;
        guarantee.dateModified = new Date();
        
        // Оновлюємо статистику і фільтрацію
        this.calculateStatusStats();
        
        // Якщо фільтр активний і гарантія більше не відповідає фільтру, оновлюємо вид
        if (this.statusFilter && this.statusFilter !== newStatus) {
          this.applyFiltering();
        }
      },
      error: (err) => {
        console.error('Помилка при оновленні статусу гарантії:', err);
        alert('Не вдалося оновити статус гарантії. Спробуйте ще раз.');
      }
    });
  }
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
}
