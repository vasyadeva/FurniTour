import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserClaim } from '../../models/userclaim.model';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { CookieService } from 'ngx-cookie-service';
import { AppStatusService } from './app.status.service';
import { ProfileModel } from '../../models/profile.model';
import { ProfileChangeModel } from '../../models/profile.change.model';
import { api } from '../../app.api';
import { CredentialsModel } from '../../models/credentials.model';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  api : string = api;
  
  constructor(private http: HttpClient,private cookieService: CookieService, private status: AppStatusService) {}

  public signIn(username: string, password: string) {
      return this.http.post<Response>(this.api +'/auth/signin', {
          username: username,
          password: password
      },{withCredentials: true});
  }

  public getUserRole(): Observable<string> {
    return this.http.get<{ role: string }>(this.api + '/auth/getrole', { withCredentials: true }).pipe(
      map(response => response.role),
      catchError(() => {
        return of('');
      })
    );
  }

  public isAdmin(): boolean {
    return localStorage.getItem('role') === 'Administrator';
  }

  public signOut() {
      return this.http.get(this.api +'/auth/signout', { withCredentials: true });
  }
  public getProfile(): Observable<ProfileModel> {
    return this.http.get<ProfileModel>(this.api +'/auth/profile', { withCredentials: true });
  }

  public changeProfile(profile: ProfileChangeModel)
  {
    return this.http.post(this.api +'/auth/changeprofile', profile, { withCredentials: true });
  }
  public user() {
      return this.http.get<UserClaim[]>(this.api +'/auth/user',  { withCredentials: true });
  }
  public credentials() {
      return this.http.get<CredentialsModel>(this.api +'/auth/credentials', { withCredentials: true });
  }
  public isSignedIn(): Observable<boolean> {
      return this.user().pipe(
          map((userClaims) => {
              const hasClaims = userClaims.length > 0;
              return !hasClaims ? false : true;
          }),
          catchError((error) => {
              return of(false);
          }));
  }
  public register(username: string, password: string, isMaster: boolean) {
      return this.http.post(this.api +'/auth/register',{username: username, password: password, isMaster: isMaster}
         ,{ withCredentials: true });
  }
}
