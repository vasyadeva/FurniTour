import { MasterReviewModel } from './master.reviews.model';
export interface ProfileMasterModel {
    username: string;
    email: string;
    phonenumber: string;
    reviews: MasterReviewModel[];
  }