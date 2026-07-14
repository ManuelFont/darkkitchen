import { Routes } from '@angular/router';
import { roleGuard } from '../../core/guards/role.guard';
import { Roles, RoleRouteData } from '../../core/auth/role.model';

const customerOnly: RoleRouteData = { roles: [Roles.Customer] };

export const ordersRoutes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () =>
      import('./order-history/order-history.component').then((m) => m.OrderHistoryComponent),
  },
  {
    path: 'new',
    canActivate: [roleGuard],
    data: customerOnly,
    loadComponent: () => import('./new-order/new-order.component').then((m) => m.NewOrderComponent),
  },
  {
    path: 'checkout',
    canActivate: [roleGuard],
    data: customerOnly,
    loadComponent: () => import('./checkout/checkout.component').then((m) => m.CheckoutComponent),
  },
  {
    path: ':id',
    loadComponent: () =>
      import('./order-detail/order-detail.component').then((m) => m.OrderDetailComponent),
  },
];
