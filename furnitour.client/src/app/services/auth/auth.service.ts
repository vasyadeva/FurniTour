import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserClaim } from '../../models/userclaim.model';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { CookieService } from 'ngx-cookie-service';
import { AppStatusService } from './app.status.service';
import { ProfileModel } from '../../models/profile.model';
import { ProfileChangeModel } from '../../models/profile.change.model';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  api : string = "https://localhost:7043/";
  
  constructor(private http: HttpClient,private cookieService: CookieService, private status: AppStatusService) {}

  public signIn(username: string, password: string) {
      return this.http.post<Response>(this.api +'api/auth/signin', {
          username: username,
          password: password
      },{withCredentials: true});
  }

  public getUserRole(): Observable<string> {
    return this.http.get<{ role: string }>(this.api + 'api/auth/getrole', { withCredentials: true }).pipe(
      map(response => response.role),
      catchError(() => {
        return of('');
      })
    );
  }


  public signOut() {
      return this.http.get(this.api +'api/auth/signout', { withCredentials: true });
  }
  public getProfile(): Observable<ProfileModel> {
    return this.http.get<ProfileModel>(this.api +'api/auth/profile', { withCredentials: true });
  }

  public changeProfile(profile: ProfileChangeModel)
  {
    return this.http.post(this.api +'api/auth/changeprofile', profile, { withCredentials: true });
  }
  public user() {
      return this.http.get<UserClaim[]>(this.api +'api/auth/user',  { withCredentials: true });
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
      return this.http.post(this.api +'api/auth/register',{username: username, password: password, isMaster: isMaster}
         ,{ withCredentials: true });
  }
}
