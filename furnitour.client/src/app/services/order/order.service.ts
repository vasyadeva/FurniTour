import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { OrderModel } from '../../models/order.model';
import { CreateOrderModel } from '../../models/create.order.model';
import { api } from '../../../environments/app.environment';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  api : string = api+"/Order/";
  constructor(private http: HttpClient) { }

  myorders():Observable<OrderModel[]> {
      return this.http.get<OrderModel[]>(this.api +'myorders', { withCredentials: true });
  }

  adminorders(): Observable<OrderModel[]> {
      return this.http.get<OrderModel[]>(this.api +'adminorders', { withCredentials: true });
  }

  add(order: CreateOrderModel) {
      return this.http.post(this.api +'add', order, { withCredentials: true });
  }

  update(id: number, state: number) {
      return this.http.post(this.api +'update', {id:id, state:state}, { withCredentials: true });
  }
}
