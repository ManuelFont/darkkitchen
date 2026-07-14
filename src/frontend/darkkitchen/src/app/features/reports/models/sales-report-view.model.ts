export interface AmountCard {
  readonly label: string;
  readonly icon: string;
  readonly value: string;
}

export interface SalesAmountsView {
  readonly items: string;
  readonly delivery: string;
  readonly iva: string;
  readonly total: string;
}

export interface ClientSalesRow {
  readonly clientId: string;
  readonly clientName: string;
  readonly amounts: SalesAmountsView;
}

export interface MonthlySalesView {
  readonly period: string;
  readonly label: string;
  readonly clients: ClientSalesRow[];
  readonly subtotal: SalesAmountsView;
}
