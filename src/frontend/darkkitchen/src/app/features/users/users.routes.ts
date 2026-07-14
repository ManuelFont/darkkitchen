import { Routes } from '@angular/router';

export const usersRoutes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./user-list/user-list.component').then((m) => m.UserListComponent),
  },
];
