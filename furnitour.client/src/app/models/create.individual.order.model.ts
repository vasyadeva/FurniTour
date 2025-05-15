export interface CreateIndividualOrderModel {
  name: string;
  address: string;
  phone: string;
  description: string;
  photo?: File;
  priceCategoryId: number;
  masterId: string;
}
