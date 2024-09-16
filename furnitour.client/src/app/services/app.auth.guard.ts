import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthService } from './auth.service';
import { Observable, of } from 'rxjs';
import { map, switchMap, catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) { }

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    const expectedRoles = next.data['roles'] as Array<string>;

    return this.authService.isSignedIn().pipe(
      switchMap(isSignedIn => {
        if (!isSignedIn) {
          this.router.navigate(['login']);
          return of(false);
        }
        return this.authService.getUserRole().pipe(
          map(role => {
            console.log('User role:', role); 
            if (expectedRoles.includes(role)) {
              return true;
            } else {
              this.router.navigate(['forbidden']);
              return false;
            }
          }),
          catchError(() => {
            this.router.navigate(['forbidden']);
            return of(false);
          })
        );
      }),
      catchError(() => {
        this.router.navigate(['forbidden']);
        return of(false);
      })
    );
  }
}
