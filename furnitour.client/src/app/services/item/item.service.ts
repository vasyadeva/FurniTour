import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { itemSend } from '../../models/item.send.model';
import { Observable } from 'rxjs';
import { itemGet } from '../../models/item.get.model';
import { itemUpdate } from '../../models/item.update.model';
import { api } from '../../../environments/app.environment';
import { ColorModel } from '../../models/color.model';
import { CategoryModel } from '../../models/category.model';
import { ItemFilterModel }  from '../../models/item.filter.model';
import { AddFurnitureReview, FurnitureReview } from '../../models/furniture.review.model';

@Injectable({
  providedIn: 'root'
})
export class ItemService {
  api : string = api+"/Item/";
  reviewApi : string = api+"/FurnitureReview/";
  
  constructor(private http: HttpClient) { }

  create(item: itemSend) {
      return this.http.post(this.api +'create', item, { withCredentials: true });
  }

  getAllItems(): Observable<itemGet[]> {
    return this.http.get<itemGet[]>(this.api + 'getall', { withCredentials: true });
  }

  getFilteredItems(filterModel: ItemFilterModel): Observable<itemGet[]> {
    return this.http.post<itemGet[]>(this.api + 'getfiltereditems', filterModel, { withCredentials: true });
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

  getManufacturers(): Observable<any[]> {
    return this.http.get<any[]>(api + '/Manufacturer/getall', { withCredentials: true });
  }

  getMasters(): Observable<any[]> {
    return this.http.get<any[]>(api + '/auth/master/getall', { withCredentials: true });
  }

  getItemByDescription(description: string): Observable<itemGet[]> {
    return this.http.get<itemGet[]>(`${this.api}/search/${description}`, { withCredentials: true });
  }
    // New methods for reviews and photos
  getItemReviews(itemId: number): Observable<FurnitureReview[]> {
    return this.http.get<FurnitureReview[]>(`${this.api}reviews/${itemId}`, { withCredentials: true });
  }
    getReviewSummary(itemId: number): Observable<string> {
    console.log('Fetching review summary for item:', itemId);
    return this.http.get(`${this.api}reviews/summary/${itemId}`, { 
      withCredentials: true,
      responseType: 'text' // Ensure we get plain text response
    });
  }
    addReview(review: AddFurnitureReview) {
    return this.http.post(`${this.api}reviews`, review, { withCredentials: true });
  }
  
  getAdditionalImageUrl(photoId: number): string {
    return `${this.api}additionalImage/${photoId}`;
  }
  countAdditionalPhotos(itemId: number): Observable<any> {
    return this.http.get<any>(`${this.api}count-photos/${itemId}`, { withCredentials: true });
  }
}
