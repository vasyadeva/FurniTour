import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { GuaranteeService } from '../../services/guarantee/guarantee.service';
import { GuaranteeModel } from '../../models/guarantee.model';
import { AuthService } from '../../services/auth/auth.service';
import { AppStatusService } from '../../services/auth/app.status.service';

@Component({
  selector: 'app-guarantee-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './guarantee-detail.component.html',
  styleUrls: ['./guarantee-detail.component.css']
})
export class GuaranteeDetailComponent implements OnInit {
  guarantee: GuaranteeModel | null = null;
  loading: boolean = true;
  error: string | null = null;
  isAdmin: boolean = false;
  currentPhotoIndex: number = 0;
  
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private guaranteeService: GuaranteeService,
    public status: AppStatusService,
    private authService: AuthService
  ) { }
  
  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();
    
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadGuarantee(+id);
    } else {
      this.error = 'Невірний ID гарантії';
      this.loading = false;
    }
  }
  
  loadGuarantee(id: number): void {
    this.guaranteeService.getGuaranteeById(id).subscribe({
      next: (data) => {
        this.guarantee = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Помилка при завантаженні деталей гарантії:', err);
        if (err.status === 403) {
          this.error = 'У вас немає доступу до цієї гарантії.';
        } else if (err.status === 404) {
          this.error = 'Гарантію не знайдено. Можливо, вона була видалена.';
        } else {
          this.error = 'Не вдалося завантажити деталі гарантії. Спробуйте пізніше.';
        }
        this.loading = false;
      }
    });
  }
  
  updateStatus(status: string): void {
    if (!this.guarantee) return;
    
    this.loading = true;
    this.guaranteeService.updateGuaranteeStatus(this.guarantee.id, status).subscribe({
      next: () => {
        if (this.guarantee) {
          this.guarantee.status = status;
          this.guarantee.dateModified = new Date();
        }
        this.loading = false;
      },
      error: (err) => {
        console.error('Помилка при оновленні статусу:', err);
        this.error = 'Не вдалося оновити статус гарантії. Спробуйте пізніше.';
        this.loading = false;
      }
    });
  }
  
  getStatusClass(status: string): string {
    switch (status) {
      case 'Очікує розгляду':
        return 'badge bg-warning text-dark';
      case 'Підтверджено':
        return 'badge bg-success';
      case 'Відхилено':
        return 'badge bg-danger';
      case 'Скасовано':
        return 'badge bg-danger';
      case 'На розгляді':
        return 'badge bg-info';
      case 'В обробці':
        return 'badge bg-primary';
      case 'Завершено':
        return 'badge bg-secondary';
      default:
        return 'badge bg-secondary';
    }
  }
  
  currentPhoto(): string {
    if (!this.guarantee || !this.guarantee.photos || this.guarantee.photos.length === 0) {
      return '';
    }
    return this.guarantee.photos[this.currentPhotoIndex];
  }
  
  previousPhoto(): void {
    if (this.currentPhotoIndex > 0) {
      this.currentPhotoIndex--;
    }
  }
  
  nextPhoto(): void {
    if (this.guarantee && this.guarantee.photos && this.currentPhotoIndex < this.guarantee.photos.length - 1) {
      this.currentPhotoIndex++;
    }
  }
  
  selectPhoto(index: number): void {
    this.currentPhotoIndex = index;
  }
  
  goBack(): void {
    this.router.navigate(['/guarantees']);
  }
}
