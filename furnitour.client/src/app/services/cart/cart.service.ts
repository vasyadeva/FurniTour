import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CartGet } from '../../models/cart.get.model';
import { api } from '../../app.api';
@Injectable({
  providedIn: 'root'
})
export class CartService {
  api : string = api+"/Cart/";
  constructor(private http: HttpClient) { }

  AddToCart(id: number, quantity: number=1) {
      return this.http.post(this.api +'add', {id, quantity}, { withCredentials: true });
  }

  getCart(): Observable<CartGet[]> {
      return this.http.get<CartGet[]>(this.api +'getcart',  { withCredentials: true });
  }

  updateQuantity(itemId: number, quantity: number) {
      return this.http.post(this.api +'update', {id:itemId, quantity}, { withCredentials: true });
  }

  removeItem(itemId: number) {
      return this.http.delete(this.api +'delete/' + itemId, { withCredentials: true });
  }
}
