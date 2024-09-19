import { OrderItemModel } from './order.item.model';
export interface OrderModel {
    id: number;
    name: string;
    address?: string;
    phone?: string;
    comment?: string;
    dateCreated: Date;
    orderState: string;
    price: number;
    orderItems: OrderItemModel[];
  }