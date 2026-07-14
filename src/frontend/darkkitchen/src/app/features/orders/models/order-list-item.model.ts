export interface OrderListItemProduct {
  productName: string;
  quantity: number;
  price: number;
}

export interface OrderListItem {
  orderId: string;
  clientFirstName: string;
  clientLastName: string;
  clientEmail: string;
  createdAt: string;
  status: string;
  total: number;
  items: OrderListItemProduct[];
}
