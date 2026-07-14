export interface CreateOrderResponse {
  orderId: string;
  clientId: string;
  subtotal: number;
  deliveryCost: number;
  total: number;
}
