import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { catchError, map } from 'rxjs/operators';

export interface User {
  id: string;
  userName: string;
  email: string;
  role: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = `${environment.apiUrl}/api/auth`;
  private userSubject = new BehaviorSubject<User | null>(null);
  user$ = this.userSubject.asObservable();
  private tokenKey = 'auth_token';
  private userIdKey = 'user_id';

  constructor(private http: HttpClient) {
    this.loadUser();
  }

  private loadUser(): void {
    const userId = localStorage.getItem(this.userIdKey);
    if (userId) {
      this.getCurrentUser().subscribe();
    }
  }

  login(email: string, password: string): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/login`, { email, password })
      .pipe(
        tap(response => {
          if (response && response.userId) {
            localStorage.setItem(this.userIdKey, response.userId);
            this.getCurrentUser().subscribe();
          }
        })
      );
  }

  register(userData: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/register`, userData);
  }

  logout(): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/logout`, {})
      .pipe(
        tap(() => {
          localStorage.removeItem(this.userIdKey);
          this.userSubject.next(null);
        }),
        catchError(error => {
          localStorage.removeItem(this.userIdKey);
          this.userSubject.next(null);
          return of(error);
        })
      );
  }

  getCurrentUser(): Observable<User | null> {
    return this.http.get<User>(`${this.baseUrl}/user`)
      .pipe(
        tap(user => {
          this.userSubject.next(user);
          localStorage.setItem(this.userIdKey, user.id);
        }),
        catchError(() => {
          localStorage.removeItem(this.userIdKey);
          this.userSubject.next(null);
          return of(null);
        })
      );
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem(this.userIdKey);
  }

  getUserId(): string {
    return localStorage.getItem(this.userIdKey) || '';
  }

  getUserRole(): Observable<string> {
    return this.user$.pipe(
      map(user => user?.role || '')
    );
  }
} 