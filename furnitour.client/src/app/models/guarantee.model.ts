export interface GuaranteeModel {
  id: number;
  userName: string;  // Required property missing from ViewModel
  orderId: number;
  status: string;
  comment: string;
  dateCreated: Date;
  dateModified: Date; // Required property missing from ViewModel
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
  orderId: number;
  comment: string;
  photos: string[];  // base64-encoded images
  items: any[];   // Change to handle different item formats
}

// Модель для відображення інформації про гарантію
export interface GuaranteeViewModel {
  id: number;
  orderId: number;
  dateCreated: Date;
  status: string;
  comment: string;
  photos?: string[];
  items?: any[];
  // Add missing fields to match GuaranteeModel
  userName?: string;
  dateModified?: Date;
}
