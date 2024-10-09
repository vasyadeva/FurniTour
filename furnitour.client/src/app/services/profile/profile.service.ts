import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProfileMasterModel } from '../../models/profile.master.model';
import { MasterAddReviewModel } from '../../models/master.add.review.model';
@Injectable({
  providedIn: 'root'
})
export class ProfileService {

  constructor(private http: HttpClient) { }
  api : string = "https://localhost:7043/api/Profile/";

  getMasterProfile(id: string): Observable<ProfileMasterModel>
  {
    return this.http.get<ProfileMasterModel>(this.api + 'getmaster/' + id, { withCredentials: true });
  }

  addMasterReview(model: MasterAddReviewModel)
  {
    return this.http.post(this.api + 'addmaster', model, { withCredentials: true });
  }
}
