import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { itemSend } from '../../models/item.send.model';
import { Observable } from 'rxjs';
import { itemGet } from '../../models/item.get.model';
import { itemUpdate } from '../../models/item.update.model';
import { api } from '../../app.api';
import {  ColorModel } from '../../models/color.model';
import { CategoryModel } from '../../models/category.model';
@Injectable({
  providedIn: 'root'
})
export class ItemService {
  api : string = api+"/Item/";
  constructor(private http: HttpClient) { }

  create(item: itemSend) {
      return this.http.post(this.api +'create', item, { withCredentials: true });
  }

  getAllItems(): Observable<itemGet[]> {
    return this.http.get<itemGet[]>(this.api + 'getall', { withCredentials: true });
  }

  delete(id: number) {
    return this.http.delete(this.api + 'delete/' + id, { withCredentials: true });
  }

  details(id: number): Observable<itemGet> {
    return this.http.get<itemGet>(this.api + 'details/' + id, { withCredentials: true });
  }

  update(item: itemUpdate) {
    return this.http.post(this.api + 'edit/', item, { withCredentials: true });
  }

  recomended(): Observable<itemGet[]> {
    return this.http.get<itemGet[]>(this.api + 'recommend', { withCredentials: true });
  }

  getColors(): Observable<ColorModel[]>
  {
    return this.http.get<ColorModel[]>(this.api + 'colors/getall', { withCredentials: true });
  }

  getCategories(): Observable<CategoryModel[]>
  {
    return this.http.get<CategoryModel[]>(this.api + 'categories/getall', { withCredentials: true });
  }

  getItemByDescription(description: string): Observable<itemGet> {
    return this.http.get<itemGet>(this.api + 'search/' + description, { withCredentials: true });
  }

}
