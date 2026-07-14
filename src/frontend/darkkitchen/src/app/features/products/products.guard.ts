import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { Roles } from '../../core/auth/role.model';

export const productsGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.hasRole(Roles.Administrator, Roles.Customer)
    ? true
    : router.parseUrl('/home');
};
