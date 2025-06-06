import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { api } from '../../../environments/app.environment';

export interface ManufacturerAdminModel {
  id: number;
  name: string;
}

export interface ManufacturerCreateModel {
  name: string;
}

export interface ManufacturerUpdateModel {
  id: number;
  name: string;
}

export interface UserAdminModel {
  id: string;
  userName: string;
  email: string;
  phoneNumber: string;
  role: string;
}

export interface UserRoleUpdateModel {
  userId: string;
  roleId: string;
}

export interface RoleModel {
  id: string;
  name: string;
  normalizedName: string;
  concurrencyStamp: string;
}

export interface ColorModel {
  id: number;
  name: string;
}

export interface ColorCreateModel {
  name: string;
}

export interface ColorUpdateModel {
  id: number;
  name: string;
}

export interface CategoryModel {
  id: number;
  name: string;
}

export interface CategoryCreateModel {
  name: string;
}

export interface CategoryUpdateModel {
  id: number;
  name: string;
}

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private readonly apiUrl = `${api}/Admin`;

  constructor(private http: HttpClient) { }

  // Manufacturer management
  getAllManufacturers(): Observable<ManufacturerAdminModel[]> {
    return this.http.get<ManufacturerAdminModel[]>(`${this.apiUrl}/manufacturers`, { withCredentials: true });
  }

  getManufacturerById(id: number): Observable<ManufacturerAdminModel> {
    return this.http.get<ManufacturerAdminModel>(`${this.apiUrl}/manufacturers/${id}`, { withCredentials: true });
  }

  createManufacturer(model: ManufacturerCreateModel): Observable<any> {
    return this.http.post(`${this.apiUrl}/manufacturers`, model, { withCredentials: true });
  }

  updateManufacturer(model: ManufacturerUpdateModel): Observable<any> {
    return this.http.put(`${this.apiUrl}/manufacturers`, model, { withCredentials: true });
  }

  deleteManufacturer(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/manufacturers/${id}`, { withCredentials: true });
  }
  isManufacturerInUse(id: number): Observable<{ inUse: boolean }> {
    return this.http.get<{ inUse: boolean }>(`${this.apiUrl}/manufacturers/${id}/in-use`, { withCredentials: true });
  }

  // User management
  getAllUsers(): Observable<UserAdminModel[]> {
    return this.http.get<UserAdminModel[]>(`${this.apiUrl}/users`, { withCredentials: true });
  }

  getUserById(id: string): Observable<UserAdminModel> {
    return this.http.get<UserAdminModel>(`${this.apiUrl}/users/${id}`, { withCredentials: true });
  }

  updateUserRole(model: UserRoleUpdateModel): Observable<any> {
    return this.http.put(`${this.apiUrl}/users/role`, model, { withCredentials: true });
  }

  canDeleteUser(id: string): Observable<{ canDelete: boolean }> {
    return this.http.get<{ canDelete: boolean }>(`${this.apiUrl}/users/${id}/can-delete`, { withCredentials: true });
  }

  deleteUser(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/users/${id}`, { withCredentials: true });
  }

  getAllRoles(): Observable<RoleModel[]> {
    return this.http.get<RoleModel[]>(`${this.apiUrl}/roles`, { withCredentials: true });
  }

  // Color management
  getAllColors(): Observable<ColorModel[]> {
    return this.http.get<ColorModel[]>(`${this.apiUrl}/colors`, { withCredentials: true });
  }

  getColorById(id: number): Observable<ColorModel> {
    return this.http.get<ColorModel>(`${this.apiUrl}/colors/${id}`, { withCredentials: true });
  }

  createColor(model: ColorCreateModel): Observable<ColorModel> {
    return this.http.post<ColorModel>(`${this.apiUrl}/colors`, model, { withCredentials: true });
  }

  updateColor(model: ColorUpdateModel): Observable<ColorModel> {
    return this.http.put<ColorModel>(`${this.apiUrl}/colors`, model, { withCredentials: true });
  }

  deleteColor(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/colors/${id}`, { withCredentials: true });
  }

  isColorInUse(id: number): Observable<{ inUse: boolean }> {
    return this.http.get<{ inUse: boolean }>(`${this.apiUrl}/colors/${id}/in-use`, { withCredentials: true });
  }

  // Category management
  getAllCategories(): Observable<CategoryModel[]> {
    return this.http.get<CategoryModel[]>(`${this.apiUrl}/categories`, { withCredentials: true });
  }

  getCategoryById(id: number): Observable<CategoryModel> {
    return this.http.get<CategoryModel>(`${this.apiUrl}/categories/${id}`, { withCredentials: true });
  }

  createCategory(model: CategoryCreateModel): Observable<any> {
    return this.http.post(`${this.apiUrl}/categories`, model, { withCredentials: true });
  }

  updateCategory(model: CategoryUpdateModel): Observable<any> {
    return this.http.put(`${this.apiUrl}/categories`, model, { withCredentials: true });
  }

  deleteCategory(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/categories/${id}`, { withCredentials: true });
  }

  isCategoryInUse(id: number): Observable<{ inUse: boolean }> {
    return this.http.get<{ inUse: boolean }>(`${this.apiUrl}/categories/${id}/in-use`, { withCredentials: true });
  }
}
