import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { Roles } from '../../core/auth/role.model';

export const productDetailsGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.hasRole(Roles.Administrator) ? true : router.parseUrl('/products');
};
