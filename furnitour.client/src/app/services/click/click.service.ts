import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { api } from '../../../environments/app.environment';
@Injectable({
  providedIn: 'root'
})
export class ClickService {
  api : string = api+"/Click/";
  constructor(private http: HttpClient) { }

  sendClick(id: number) {
    return this.http.post(this.api + 'add', { itemId: id }, { withCredentials: true });
  }
}
