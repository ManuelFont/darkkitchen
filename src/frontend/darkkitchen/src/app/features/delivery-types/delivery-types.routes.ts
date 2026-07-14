import { Routes } from '@angular/router';

export const deliveryTypesRoutes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () =>
      import('./delivery-type-list/delivery-type-list.component').then(
        (m) => m.DeliveryTypeListComponent,
      ),
  },
];
