import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProfileMasterModel } from '../../models/profile.master.model';
import { MasterAddReviewModel } from '../../models/master.add.review.model';
import { ProfileManufacturerModel } from '../../models/profile.manufacturer.model';
import { ManufacturerAddReviewModel } from '../../models/manufacturer.add.review.model';
import { api } from '../../app.api';
import { AIReviewModel } from '../../models/ai.review.model';
import { ProfileModel } from '../../models/profile.model';
@Injectable({
  providedIn: 'root'
})
export class ProfileService {

  constructor(private http: HttpClient) { }
  api : string = api + "/Profile/";

  getMasterProfile(id: string): Observable<ProfileMasterModel>
  {
    return this.http.get<ProfileMasterModel>(this.api + 'getmaster/' + id, { withCredentials: true });
  }

  getManufacturerProfile(name: string): Observable<ProfileManufacturerModel>
  {
    return this.http.get<ProfileManufacturerModel>(this.api + 'getmanufacturer/' + name, { withCredentials: true });
  }

  getMasterAIReview(id: string): Observable<AIReviewModel>
  {
    return this.http.get<AIReviewModel>(this.api + 'ai/master/review/' + id, { withCredentials: true });
  }

  getManufacturerAIReview(id: string): Observable<AIReviewModel>
  {
    return this.http.get<AIReviewModel>(this.api + 'ai/manufacturer/review/' + id, { withCredentials: true });
  }

  addMasterReview(model: MasterAddReviewModel)
  {
    return this.http.post(this.api + 'addmaster', model, { withCredentials: true });
  }

  addManufacturerReview(model: ManufacturerAddReviewModel)
  {
    return this.http.post(this.api + 'addmanufacturer', model, { withCredentials: true });
  }
  profile(): Observable<ProfileModel> {
    return this.http.get<ProfileModel>(api + '/auth/profile', { withCredentials: true });
  }

  // Get public profile by username
  getPublicProfile(username: string): Observable<ProfileModel> {
    return this.http.get<ProfileModel>(this.api + 'public/' + username, { withCredentials: true });
  }
}
