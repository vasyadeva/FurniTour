export interface IndividualOrderModel {
  id: number;
  name: string;
  address: string;
  phone?: string;
  dateCreated: Date;
  description: string;
  photo: string;
  estimatedPrice?: number;
  finalPrice?: number;
  masterNotes?: string;
  priceCategory: string;
  status: string;
  dateCompleted?: Date;
  masterName: string;
  userName: string;
}
