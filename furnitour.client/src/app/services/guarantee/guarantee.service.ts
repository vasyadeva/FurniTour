import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { GuaranteeAddModel, GuaranteeViewModel, GuaranteeModel } from '../../models/guarantee.model';
import { OrderService } from '../order/order.service';
import { AuthService } from '../auth/auth.service';
import { api } from '../../app.api';

@Injectable({
  providedIn: 'root'
})
export class GuaranteeService {
  apiUrl: string = api;
  guaranteeApi: string = `${api}/Guarantee/`;
    constructor(
    private http: HttpClient,
    private orderService: OrderService,
    private authService: AuthService
  ) { }

  // Get guarantees for the current user
  getUserGuarantees(): Observable<GuaranteeModel[]> {
    return this.http.get<GuaranteeModel[]>(`${this.guaranteeApi}my`, { withCredentials: true })
      .pipe(
        map(guarantees => {
          console.log('Received guarantees:', guarantees);
          return guarantees;
        }),
        catchError(error => {
          console.error('Error fetching guarantees:', error);
          return throwError(() => new Error('Не вдалося отримати список гарантій'));
        })
      );
  }

  // For backward compatibility - alias to getUserGuarantees
  getMyGuarantees(): Observable<GuaranteeModel[]> {
    return this.getUserGuarantees();
  }
  // Get all guarantees (admin only)
  getAllGuarantees(): Observable<GuaranteeModel[]> {
    return this.http.get<GuaranteeModel[]>(`${this.guaranteeApi}getall`, { withCredentials: true })
      .pipe(
        catchError(error => {
          console.error('Error fetching all guarantees:', error);
          return throwError(() => new Error('Не вдалося отримати список усіх гарантій'));
        })
      );
  }

  // Отримання замовлень користувача, доступних для гарантії
  getUserOrders(): Observable<any[]> {
    return this.orderService.myorders()
      .pipe(
        map(orders => {
          console.log('Отримані замовлення з OrderService:', orders);
          return orders;
        }),
        catchError(error => {
          console.error('Error fetching available orders:', error);
          return throwError(() => new Error('Не вдалося отримати список замовлень'));
        })
      );
  }
  // Отримання деталей конкретного замовлення
  getOrderDetails(orderId: string | number): Observable<any> {
    return this.orderService.myorders()
      .pipe(
        map(orders => {
          // Знаходимо замовлення по ID
          const order = orders.find(o => String(o.id) === String(orderId));
          
          if (!order) {
            throw new Error(`Замовлення з ID ${orderId} не знайдено`);
          }
          
          console.log('Знайдено замовлення:', order);
          return order;
        }),
        catchError(error => {
          console.error(`Error fetching order details for ID ${orderId}:`, error);
          return throwError(() => new Error('Не вдалося отримати деталі замовлення'));
        })
      );
  }
  // Додавання нової гарантії
  addGuarantee(guaranteeRequest: any): Observable<any> {
    // Prepare the request based on order type
    const preparedRequest: any = {
      comment: guaranteeRequest.comment,
      photos: guaranteeRequest.photos || [],
      isIndividualOrder: !!guaranteeRequest.isIndividualOrder
    };
    
    if (preparedRequest.isIndividualOrder) {
      preparedRequest.individualOrderId = Number(guaranteeRequest.individualOrderId);
    } else {
      preparedRequest.orderId = Number(guaranteeRequest.orderId);
      preparedRequest.items = guaranteeRequest.items || []; // Do not modify the item IDs
    }
    
    console.log('Sending guarantee request:', JSON.stringify(preparedRequest));
    
    // Send to the API endpoint
    return this.http.post<any>(`${this.guaranteeApi}add`, preparedRequest, { withCredentials: true })
      .pipe(
        catchError(error => {
          console.error('Error submitting guarantee request:', error);
          return throwError(() => new Error('Не вдалося створити запит на гарантію'));
        })
      );
  }

  // Update guarantee status - missing method causing errors
  updateGuaranteeStatus(guaranteeId: number, status: string): Observable<any> {
    return this.http.post<any>(`${this.guaranteeApi}update/${guaranteeId}`, JSON.stringify(status), {
      headers: { 'Content-Type': 'application/json' },
      withCredentials: true
    }).pipe(
      catchError(error => {
        console.error('Error updating guarantee status:', error);
        return throwError(() => new Error('Не вдалося оновити статус гарантії'));
      })
    );
  }  // Отримання деталей гарантії за ID
  getGuaranteeById(id: number): Observable<GuaranteeModel> {
    const isAdmin = this.authService.isAdmin();
    const endpoint = isAdmin ? `admin/details/${id}` : `get/${id}`;
    
    return this.http.get<GuaranteeModel>(`${this.guaranteeApi}${endpoint}`, { withCredentials: true })
      .pipe(
        catchError(error => {
          console.error(`Error fetching guarantee details for ID ${id}:`, error);
          return throwError(() => new Error('Не вдалося отримати деталі гарантії'));
        })
      );
  }

  // Отримання всіх можливих статусів для гарантії
  getAvailableStatuses(): Observable<string[]> {
    return this.http.get<string[]>(`${this.guaranteeApi}statuses`, { withCredentials: true })
      .pipe(
        catchError(error => {
          console.error('Помилка при отриманні списку статусів:', error);
          return throwError(() => new Error('Не вдалося отримати список статусів гарантії'));
        })
      );
  }

  // Допоміжний метод для конвертації файла в Base64
  async convertFileToBase64(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => {
        if (typeof reader.result === 'string') {
          const base64String = reader.result.split(',')[1];
          resolve(base64String);
        } else {
          reject('Не вдалося конвертувати файл в base64');
        }
      };
      reader.onerror = error => reject(error);
    });
  }

  // Отримання статистики щодо гарантій для адміністратора
  getGuaranteeStatistics(): Observable<any> {
    return this.http.get<any>(`${this.guaranteeApi}admin/statistics`, { withCredentials: true })
      .pipe(
        catchError(error => {
          console.error('Помилка при отриманні статистики гарантій:', error);
          return throwError(() => new Error('Не вдалося отримати статистику гарантій'));
        })
      );
  }

  // Фільтрація гарантій для адміністратора (за статусом, користувачем, датою)
  getFilteredGuarantees(
    statusFilter?: string, 
    userFilter?: string, 
    dateFromFilter?: string, 
    dateToFilter?: string
  ): Observable<GuaranteeModel[]> {
    let params: any = {};
    
    if (statusFilter) params.status = statusFilter;
    if (userFilter) params.user = userFilter;
    if (dateFromFilter) params.dateFrom = dateFromFilter;
    if (dateToFilter) params.dateTo = dateToFilter;
    
    return this.http.get<GuaranteeModel[]>(`${this.guaranteeApi}admin/filter`, { 
      params,
      withCredentials: true 
    }).pipe(
      catchError(error => {
        console.error('Помилка при фільтрації гарантій:', error);
        return throwError(() => new Error('Не вдалося відфільтрувати гарантії'));
      })
    );
  }
  
  // Експорт гарантій в CSV (адмін)
  exportGuaranteesToCsv(
    statusFilter?: string, 
    userFilter?: string, 
    dateFromFilter?: string, 
    dateToFilter?: string
  ): Observable<Blob> {
    let params: any = {};
    
    if (statusFilter) params.status = statusFilter;
    if (userFilter) params.user = userFilter;
    if (dateFromFilter) params.dateFrom = dateFromFilter;
    if (dateToFilter) params.dateTo = dateToFilter;
    
    return this.http.get(`${this.guaranteeApi}admin/export`, { 
      params,
      responseType: 'blob',
      withCredentials: true 
    }).pipe(
      catchError(error => {
        console.error('Помилка при експорті гарантій у CSV:', error);
        return throwError(() => new Error('Не вдалося експортувати гарантії в CSV'));
      })
    );
  }
  
  // Отримання списку всіх користувачів, які створили гарантії
  getGuaranteeUsers(): Observable<string[]> {
    return this.http.get<string[]>(`${this.guaranteeApi}admin/users`, { withCredentials: true })
      .pipe(
        catchError(error => {
          console.error('Помилка при отриманні списку користувачів з гарантіями:', error);
          return throwError(() => new Error('Не вдалося отримати список користувачів'));
        })
      );
  }

  // Масове оновлення гарантій
  bulkUpdateGuarantees(updates: {id: number, status: string}[]): Observable<any> {
    return this.http.post<any>(`${this.guaranteeApi}admin/bulk-update`, updates, { 
      withCredentials: true 
    }).pipe(
      catchError(error => {
        console.error('Помилка при масовому оновленні гарантій:', error);
        return throwError(() => new Error('Не вдалося оновити гарантії'));
      })
    );  }
  // Get individual orders for the current user
  getUserIndividualOrders(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/IndividualOrder`, { withCredentials: true })
      .pipe(
        map(orders => {
          // Filter orders suitable for guarantee (e.g., completed, less than 365 days old)
          const eligibleOrders = orders.filter(order => {
            const orderDate = new Date(order.dateCreated);
            const daysAgo = (new Date().getTime() - orderDate.getTime()) / (1000 * 3600 * 24);
            return daysAgo <= 365; // Only orders less than a year old
          });
          
          console.log('Eligible individual orders for guarantee:', eligibleOrders);
          return eligibleOrders;
        }),
        catchError(error => {
          console.error('Error fetching individual orders:', error);
          return throwError(() => new Error('Не вдалося отримати список індивідуальних замовлень'));
        })
      );
  }
  
  // Get individual order details
  getIndividualOrderDetails(orderId: string | number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/IndividualOrder/${orderId}`, { withCredentials: true })
      .pipe(
        catchError(error => {
          console.error(`Error fetching individual order details for ID ${orderId}:`, error);
          return throwError(() => new Error('Не вдалося отримати деталі індивідуального замовлення'));
        })
      );
  }
}
