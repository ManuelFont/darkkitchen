import { Routes } from '@angular/router';

export const reportsRoutes: Routes = [
  {
    path: 'top-products',
    loadComponent: () =>
      import('./top-products/top-products.component').then((m) => m.TopProductsComponent),
  },
  {
    path: 'sales',
    loadComponent: () =>
      import('./sales-report/sales-report.component').then((m) => m.SalesReportComponent),
  },
  { path: '', pathMatch: 'full', redirectTo: 'top-products' },
];
