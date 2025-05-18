import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoyaltyModel } from '../../models/loyalty.model';
import { api } from '../../app.api';

@Injectable({
  providedIn: 'root'
})
export class LoyaltyService {
  private apiUrl = api + '/loyalty';

  constructor(private http: HttpClient) { }

  getUserLoyalty(): Observable<LoyaltyModel> {
    return this.http.get<LoyaltyModel>(this.apiUrl, { withCredentials: true });
  }

  getUserDiscount(): Observable<{discount: number, discountPercent: number}> {
    return this.http.get<{discount: number, discountPercent: number}>(`${this.apiUrl}/discount`, { withCredentials: true });
  }
}
