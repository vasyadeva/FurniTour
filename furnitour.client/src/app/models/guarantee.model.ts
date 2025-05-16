export interface GuaranteeModel {
  id: number;
  userName: string;
  orderId?: number;
  individualOrderId?: number;
  isIndividualOrder: boolean;
  status: string;
  comment: string;
  dateCreated: Date;
  dateModified: Date;
  items: GuaranteeItemModel[];
  photos: string[];
}

export interface GuaranteeItemModel {
  id: number;
  furnitureId: number;
  furnitureName: string;
  quantity: number;
}

// Модель для створення запиту на гарантію
export interface GuaranteeAddModel {
  orderId?: number;
  individualOrderId?: number;
  isIndividualOrder: boolean;
  comment: string;
  photos: string[];  // base64-encoded images
  items: any[];   // Change to handle different item formats
}

// Модель для відображення інформації про гарантію
export interface GuaranteeViewModel {
  id: number;
  orderId?: number;
  individualOrderId?: number;
  isIndividualOrder: boolean;
  dateCreated: Date;
  status: string;
  comment: string;
  photos?: string[];
  items?: any[];
  userName?: string;
  dateModified?: Date;
}
