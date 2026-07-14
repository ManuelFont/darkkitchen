export interface CreateOrderItemRequest {
  productId: string;
  quantity: number;
}

export interface CreateOrderRequest {
  deliveryTypeId: string;
  address: {
    street: string;
    doorNumber: number;
    apartment?: string | null;
  };
  items: CreateOrderItemRequest[];
}
