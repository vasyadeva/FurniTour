import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { IndividualOrderModel } from '../../models/individual.order.model';
import { CreateIndividualOrderModel } from '../../models/create.individual.order.model';
import { api } from '../../../environments/app.environment';
import { PriceCategoryModel } from '../../models/price.category.model';
import { MasterModel } from '../../models/master.model';

@Injectable({
  providedIn: 'root'
})
export class IndividualOrderService {
  api: string = api + "/IndividualOrder";
  
  constructor(private http: HttpClient) { }

  // Отримання списку власних індивідуальних замовлень
  getMyIndividualOrders(): Observable<IndividualOrderModel[]> {
    return this.http.get<IndividualOrderModel[]>(this.api, { withCredentials: true })
      .pipe(
        map(orders => {
          // Переконуємося, що всі URL зображень правильно сформовані
          return orders.map(order => this.processOrderImages(order));
        })
      );
  }
  // Створення нового індивідуального замовлення
  createIndividualOrder(order: CreateIndividualOrderModel): Observable<any> {
    const formData = new FormData();
    formData.append('name', order.name);
    formData.append('address', order.address);
    formData.append('phone', order.phone);
    formData.append('description', order.description);
    formData.append('priceCategoryId', order.priceCategoryId.toString());
    formData.append('masterId', order.masterId);
    
    if (order.photo) {
      formData.append('photo', order.photo);
    }

    return this.http.post(this.api, formData, { withCredentials: true });
  }

  // Отримання списку всіх індивідуальних замовлень (для адміністратора/майстра)
  getAllIndividualOrders(): Observable<IndividualOrderModel[]> {
    return this.http.get<IndividualOrderModel[]>(`${this.api}/admin`, { withCredentials: true })
      .pipe(
        map(orders => {
          // Переконуємося, що всі URL зображень правильно сформовані
          return orders.map(order => this.processOrderImages(order));
        })
      );
  }

  // Отримання деталей конкретного індивідуального замовлення
  getIndividualOrderDetails(id: number): Observable<IndividualOrderModel> {
    return this.http.get<IndividualOrderModel>(`${this.api}/${id}`, { withCredentials: true })
      .pipe(
        map(order => this.processOrderImages(order))
      );
  }

  // Зміна статусу індивідуального замовлення
  changeIndividualOrderStatus(id: number, statusId: number): Observable<any> {
    return this.http.post(`${this.api}/status/${id}/${statusId}`, {}, { withCredentials: true });
  }

  // Призначення майстра для індивідуального замовлення
  assignMasterToOrder(orderId: number, masterId: string): Observable<any> {
    return this.http.post(`${this.api}/assign/${orderId}/${masterId}`, {}, { withCredentials: true });
  }

  // Встановлення оцінки вартості для індивідуального замовлення
  setEstimatedPrice(orderId: number, price: number): Observable<any> {
    return this.http.post(`${this.api}/price/estimated/${orderId}`, price, { withCredentials: true });
  }

  // Встановлення фінальної вартості для індивідуального замовлення
  setFinalPrice(orderId: number, price: number): Observable<any> {
    return this.http.post(`${this.api}/price/final/${orderId}`, price, { withCredentials: true });
  }

  // Додавання коментаря майстра до індивідуального замовлення
  addMasterNotes(orderId: number, notes: string): Observable<any> {
    return this.http.post(`${this.api}/notes/${orderId}`, JSON.stringify(notes), { 
      withCredentials: true,
      headers: { 'Content-Type': 'application/json' }
    });
  }
  
  // Отримання списку цінових категорій
  getPriceCategories(): Observable<PriceCategoryModel[]> {
    return this.http.get<PriceCategoryModel[]>(`${api}/PriceCategory`, { withCredentials: true });
  }
  
  // Отримання списку майстрів
  getMasters(): Observable<MasterModel[]> {
    return this.http.get<MasterModel[]>(`${api}/Master`, { withCredentials: true });
  }
  // Допоміжний метод для обробки URL зображень замовлень
  private processOrderImages(order: IndividualOrderModel): IndividualOrderModel {
    // Перевіряємо, чи URL є повним або відносним
    if (order.photo && !order.photo.startsWith('http') && !order.photo.startsWith('blob:')) {
      console.log(`Перетворення відносного URL: ${order.photo}`);
      
      // Видаляємо префікс "api/" якщо він присутній, щоб уникнути дублювання
      if (order.photo.startsWith('api/')) {
        order.photo = order.photo.substring(4); // Видаляємо "api/"
      }
      
      // Якщо API використовує відносний шлях, додаємо базовий URL API
      if (!order.photo.startsWith('/')) {
        order.photo = `${api}/${order.photo}`;
      } else {
        // Якщо URL розпочинається зі слеша, використовуємо поточний хост
        const baseUrl = window.location.origin;
        order.photo = `${baseUrl}${order.photo}`;
      }
      console.log(`Повний URL зображення: ${order.photo}`);
    }
    return order;
  }
}
