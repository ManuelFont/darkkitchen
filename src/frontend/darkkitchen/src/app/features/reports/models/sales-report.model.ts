export interface SalesAmounts {
  readonly items: number;
  readonly delivery: number;
  readonly iva: number;
  readonly total: number;
}

export interface ClientSales {
  readonly clientId: string;
  readonly clientName: string;
  readonly amounts: SalesAmounts;
}

export interface MonthlySales {
  readonly period: string;
  readonly year: number;
  readonly month: number;
  readonly clients: ClientSales[];
  readonly subtotal: SalesAmounts;
}

export interface SalesReport {
  readonly months: MonthlySales[];
  readonly grandTotal: SalesAmounts;
}
