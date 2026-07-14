import { Routes } from '@angular/router';
import { noAuthGuard } from './core/guards/no-auth.guard';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
import { Roles, RoleRouteData } from './core/auth/role.model';

export const routes: Routes = [
  {
    path: 'login',
    canActivate: [noAuthGuard],
    loadChildren: () => import('./features/auth/auth.routes').then((m) => m.loginRoutes),
  },
  {
    path: 'register',
    canActivate: [noAuthGuard],
    loadChildren: () => import('./features/auth/auth.routes').then((m) => m.registerRoutes),
  },
  {
    path: 'home',
    canActivate: [authGuard],
    loadChildren: () => import('./features/home/home.routes').then((m) => m.homeRoutes),
  },
  {
    path: 'products',
    canActivate: [authGuard, roleGuard],
    data: { roles: [Roles.Administrator] } satisfies RoleRouteData,
    loadChildren: () => import('./features/products/products.routes').then((m) => m.productsRoutes),
  },
  {
    path: 'orders',
    canActivate: [authGuard, roleGuard],
    data: {
      roles: [Roles.Customer, Roles.Dispatcher, Roles.Administrator],
    } satisfies RoleRouteData,
    loadChildren: () => import('./features/orders/orders.routes').then((m) => m.ordersRoutes),
  },
  {
    path: 'users',
    canActivate: [authGuard, roleGuard],
    data: { roles: [Roles.Administrator] } satisfies RoleRouteData,
    loadChildren: () => import('./features/users/users.routes').then((m) => m.usersRoutes),
  },
  {
    path: 'promotions',
    canActivate: [authGuard, roleGuard],
    data: { roles: [Roles.Administrator] } satisfies RoleRouteData,
    loadChildren: () =>
      import('./features/promotions/promotions.routes').then((m) => m.promotionsRoutes),
  },
  {
    path: 'delivery-types',
    canActivate: [authGuard, roleGuard],
    data: { roles: [Roles.Administrator] } satisfies RoleRouteData,
    loadChildren: () =>
      import('./features/delivery-types/delivery-types.routes').then((m) => m.deliveryTypesRoutes),
  },
  {
    path: 'audit-logs',
    canActivate: [authGuard, roleGuard],
    data: { roles: [Roles.Administrator] } satisfies RoleRouteData,
    loadChildren: () =>
      import('./features/audit-logs/audit-logs.module').then((m) => m.AuditLogsModule),
  },
  {
    path: 'reports',
    canActivate: [authGuard, roleGuard],
    data: { roles: [Roles.Administrator] } satisfies RoleRouteData,
    loadChildren: () => import('./features/reports/reports.routes').then((m) => m.reportsRoutes),
  },
  {
    path: 'not-found',
    loadComponent: () =>
      import('./features/not-found/not-found.component').then((m) => m.NotFoundComponent),
  },
  {
    path: 'server-error',
    loadComponent: () =>
      import('./features/server-error/server-error.component').then((m) => m.ServerErrorComponent),
  },
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: '**', redirectTo: '/home' },
];
