import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ManufacturerModel } from '../../models/manufacturer.model';
import { AddManufacturerModel } from '../../models/add.manufacturer.model';
import { api } from '../../app.api';

@Injectable({
  providedIn: 'root'
})
export class ManufacturerService {

  constructor(private http: HttpClient) { }
  api : string = api+"/Manufacturer/";
  
  getAll(): Observable<ManufacturerModel[]> {
    return this.http.get<ManufacturerModel[]>(this.api + 'getall', { withCredentials: true });
  }

  get(id: number): Observable<ManufacturerModel> {
    return this.http.get<ManufacturerModel>(this.api + 'get/' + id, { withCredentials: true });
  }

  add(manufacturer: AddManufacturerModel) {
    return this.http.post(this.api + 'add', manufacturer, { withCredentials: true });
  }

  update(manufacturer: ManufacturerModel) {
    return this.http.post(this.api + 'update', manufacturer, { withCredentials: true });
  }

  remove(id: number) {
    return this.http.delete(this.api + 'delete/' + id, { withCredentials: true
    });}
}
