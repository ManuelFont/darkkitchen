import { Routes } from '@angular/router';
import { productDetailsGuard } from './product-details.guard';
import { productsGuard } from './products.guard';

export const productsRoutes: Routes = [
  {
    path: '',
    canActivate: [productsGuard],
    pathMatch: 'full',
    loadComponent: () =>
      import('./product-list/product-list.component').then((m) => m.ProductListComponent),
  },
  {
    path: ':id',
    canActivate: [productDetailsGuard],
    loadComponent: () =>
      import('./product-details/product-details.component').then(
        (m) => m.ProductDetailsComponent,
      ),
  },
];
