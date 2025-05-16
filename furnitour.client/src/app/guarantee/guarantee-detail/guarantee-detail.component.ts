import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { GuaranteeService } from '../../services/guarantee/guarantee.service';
import { GuaranteeModel } from '../../models/guarantee.model';
import { AuthService } from '../../services/auth/auth.service';

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
  }  loadGuarantee(id: number): void {
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
  updateStatus(newStatus: string): void {
    if (!this.guarantee || !this.isAdmin) return;
    
    this.guaranteeService.updateGuaranteeStatus(this.guarantee.id, newStatus).subscribe({
      next: () => {
        if (this.guarantee) {
          this.guarantee.status = newStatus;
          this.guarantee.dateModified = new Date();
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
  
  nextPhoto(): void {
    if (this.guarantee?.photos && this.currentPhotoIndex < this.guarantee.photos.length - 1) {
      this.currentPhotoIndex++;
    }
  }
  
  prevPhoto(): void {
    if (this.currentPhotoIndex > 0) {
      this.currentPhotoIndex--;
    }
  }
  
  goBack(): void {
    this.router.navigate(['/guarantees']);
  }
}
