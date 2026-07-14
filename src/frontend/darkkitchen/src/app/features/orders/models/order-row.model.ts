export interface OrderRow {
  orderId: string;
  shortId: string;
  createdAt: string;
  productCount: number;
  total: string;
  statusLabel: string;
  statusBadgeClass: string;
  statusDotClass: string;
  status: string;
  customerName?: string;
  customerEmail?: string;
  hasActions?: boolean;
}
