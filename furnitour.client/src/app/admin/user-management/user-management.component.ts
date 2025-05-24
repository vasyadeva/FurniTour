import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService, UserAdminModel, RoleModel, UserRoleUpdateModel } from '../../services/admin/admin.service';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users: UserAdminModel[] = [];
  filteredUsers: UserAdminModel[] = [];
  roles: RoleModel[] = [];
  searchTerm: string = '';
  selectedRoleFilter: string = '';
  isLoading: boolean = false;
  error: string = '';
  successMessage: string = '';

  constructor(private adminService: AdminService) { }

  ngOnInit(): void {
    this.loadUsers();
    this.loadRoles();
  }

  loadUsers(): void {
    this.isLoading = true;
    this.error = '';
    
    this.adminService.getAllUsers().subscribe({
      next: (users) => {
        this.users = users;
        this.filteredUsers = [...this.users];
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading users:', error);
        this.error = 'Не вдалося завантажити користувачів';
        this.isLoading = false;
      }
    });
  }

  loadRoles(): void {
    this.adminService.getAllRoles().subscribe({
      next: (roles) => {
        this.roles = roles;
      },
      error: (error) => {
        console.error('Error loading roles:', error);
      }
    });
  }

  filterUsers(): void {
    let filtered = [...this.users];

    // Filter by search term
    if (this.searchTerm.trim()) {
      const searchLower = this.searchTerm.toLowerCase();
      filtered = filtered.filter(user =>
        user.userName.toLowerCase().includes(searchLower) ||
        user.email?.toLowerCase().includes(searchLower) ||
        user.phoneNumber?.toLowerCase().includes(searchLower)
      );
    }

    // Filter by role
    if (this.selectedRoleFilter) {
      filtered = filtered.filter(user => user.role === this.selectedRoleFilter);
    }

    this.filteredUsers = filtered;
  }

  resetSearch(): void {
    this.searchTerm = '';
    this.selectedRoleFilter = '';
    this.filteredUsers = [...this.users];
  }

  updateUserRole(user: UserAdminModel, newRoleId: string): void {
    this.isLoading = true;
    this.error = '';

    const updateData: UserRoleUpdateModel = {
      userId: user.id,
      roleId: newRoleId
    };

    this.adminService.updateUserRole(updateData).subscribe({
      next: () => {
        // Update local data
        const role = this.roles.find(r => r.id === newRoleId);
        if (role) {
          user.role = role.name;
        }
        this.successMessage = 'Роль користувача успішно оновлено';
        this.isLoading = false;
        
        setTimeout(() => {
          this.successMessage = '';
        }, 3000);
      },
      error: (error) => {
        console.error('Error updating user role:', error);
        this.error = error.error || 'Не вдалося оновити роль користувача';
        this.isLoading = false;
      }
    });
  }

  deleteUser(user: UserAdminModel): void {
    if (!confirm(`Ви впевнені, що хочете видалити користувача "${user.userName}"?`)) {
      return;
    }

    this.isLoading = true;
    this.error = '';

    // First check if user can be deleted
    this.adminService.canDeleteUser(user.id).subscribe({
      next: (result) => {
        if (!result.canDelete) {
          this.error = 'Неможливо видалити користувача, оскільки він має пов\'язані дані в системі';
          this.isLoading = false;
          return;
        }

        // Proceed with deletion
        this.adminService.deleteUser(user.id).subscribe({
          next: () => {
            this.users = this.users.filter(u => u.id !== user.id);
            this.filterUsers();
            this.successMessage = 'Користувача успішно видалено';
            this.isLoading = false;
            
            setTimeout(() => {
              this.successMessage = '';
            }, 3000);
          },
          error: (error) => {
            console.error('Error deleting user:', error);
            this.error = error.error || 'Не вдалося видалити користувача';
            this.isLoading = false;
          }
        });
      },
      error: (error) => {
        console.error('Error checking user deletion:', error);
        this.error = 'Не вдалося перевірити можливість видалення користувача';
        this.isLoading = false;
      }
    });
  }

  getRoleClass(role: string): string {
    switch (role.toLowerCase()) {
      case 'administrator':
        return 'badge bg-danger';
      case 'master':
        return 'badge bg-warning';
      case 'user':
        return 'badge bg-primary';
      default:
        return 'badge bg-secondary';
    }
  }

  trackByUserId(index: number, user: UserAdminModel): string {
    return user.id;
  }

  getUserRoleId(user: UserAdminModel): string {
    const role = this.roles.find(r => r.name === user.role);
    return role ? role.id : '';
  }

  onRoleChange(user: UserAdminModel, event: Event): void {
    const target = event.target as HTMLSelectElement;
    const newRoleId = target.value;
    
    // Only update if the role actually changed
    const currentRoleId = this.getUserRoleId(user);
    if (newRoleId !== currentRoleId) {
      this.updateUserRole(user, newRoleId);
    }
  }
}
