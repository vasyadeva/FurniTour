import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AppStatusService {
  private authStatusSource = new BehaviorSubject<{
    isSignedIn: boolean;
    isAdmin: boolean;
    isMaster: boolean;
    isUser: boolean;
  }>({
    isSignedIn: false,
    isAdmin: false,
    isMaster: false,
    isUser: false
  });

  public authStatus$ = this.authStatusSource.asObservable();
  
  constructor() {
    // Check if user is already authenticated on service init
    this.loadAuthStatusFromStorage();
  }

  get isSignedIn(): boolean {
    return this.authStatusSource.value.isSignedIn;
  }

  get isAdmin(): boolean {
    return this.authStatusSource.value.isAdmin;
  }

  get isMaster(): boolean {
    return this.authStatusSource.value.isMaster;
  }

  get isUser(): boolean {
    return this.authStatusSource.value.isUser;
  }

  updateAuthStatus(status: { isSignedIn: boolean, isAdmin: boolean, isMaster: boolean, isUser: boolean }): void {
    this.authStatusSource.next(status);
    // Save to localStorage to persist between page refreshes
    localStorage.setItem('authStatus', JSON.stringify(status));
  }

  signOut(): void {
    const resetStatus = {
      isSignedIn: false,
      isAdmin: false,
      isMaster: false,
      isUser: false
    };
    this.authStatusSource.next(resetStatus);
    localStorage.removeItem('authStatus');
  }

  private loadAuthStatusFromStorage(): void {
    const savedStatus = localStorage.getItem('authStatus');
    if (savedStatus) {
      try {
        const parsedStatus = JSON.parse(savedStatus);
        this.authStatusSource.next(parsedStatus);
      } catch (e) {
        console.error('Failed to parse saved auth status', e);
        localStorage.removeItem('authStatus');
      }
    }
  }
}
