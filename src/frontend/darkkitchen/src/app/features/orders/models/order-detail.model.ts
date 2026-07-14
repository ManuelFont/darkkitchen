export interface OrderDetailItem {
  productName: string;
  quantity: number;
  unitPrice: number;
  subtotal: number;
}

export interface OrderDetail {
  orderId: string;
  clientFirstName: string;
  clientLastName: string;
  clientEmail: string;
  createdAt: string;
  status: string;
  subtotal: number;
  deliveryCost: number;
  total: number;
  deliveryType: string;
  street: string;
  doorNumber: number;
  apartment?: string | null;
  items: OrderDetailItem[];
}
