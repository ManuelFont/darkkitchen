import { Routes } from '@angular/router';

export const promotionsRoutes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () =>
      import('./promotion-list/promotion-list.component').then((m) => m.PromotionListComponent),
  },
];
