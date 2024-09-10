import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

@Injectable()
export class AuthGuard implements CanActivate {
    constructor(private authService: AuthService, private router: Router) { }

    canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        return this.isSignedIn();
    }

    isSignedIn(): Observable<boolean> {
        return this.authService.isSignedIn().pipe(
            map((isSignedIn) => {
                if (!isSignedIn) {
                    this.router.navigate(['signin']);
                    return false;
                }
                return true;
            }));
    }
}