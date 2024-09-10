import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserClaim } from '../models/auth.interface';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
@Injectable({
  providedIn: 'root',
})
export class AuthService {
  api : string = "https://localhost:7043/";
  constructor(private http: HttpClient) {}

  public signIn(username: string, password: string) {
      return this.http.post<Response>(this.api +'api/auth/signin', {
          username: username,
          password: password
      },{withCredentials: true});
  }

  public signOut() {
      return this.http.get(this.api +'api/auth/signout', { withCredentials: true });
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
  public register(username: string, password: string) {
      return this.http.post(this.api +'api/auth/register',{username: username, password: password}
         ,{ withCredentials: true });
  }
}