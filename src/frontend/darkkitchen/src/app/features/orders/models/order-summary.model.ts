export interface ClientOrderSummary {
  orderId: string;
  clientFirstName: string;
  clientLastName: string;
  clientEmail: string;
  createdAt: string;
  status: string;
  total: number;
  productCount: number;
}
