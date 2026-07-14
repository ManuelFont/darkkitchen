export interface OrderDetailItemView {
  productName: string;
  quantity: number;
  unitPriceLabel: string;
  subtotalLabel: string;
}

export interface OrderDetailView {
  shortId: string;
  createdAt: string;
  statusLabel: string;
  statusBadgeClass: string;
  statusDotClass: string;
  deliveryTypeLabel: string;
  addressLine: string;
  items: OrderDetailItemView[];
  subtotalLabel: string;
  deliveryLabel: string;
  totalLabel: string;
}
