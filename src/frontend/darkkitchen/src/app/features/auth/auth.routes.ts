import { Routes } from '@angular/router';

export const loginRoutes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./components/login.component').then((m) => m.LoginComponent),
  },
];

export const registerRoutes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./components/register.component').then((m) => m.RegisterComponent),
  },
];
