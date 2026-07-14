import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { Role } from '../auth/role.model';

/**
 * Blocks activation of a route unless the logged-in user has one of the roles
 * declared in the route's `data.roles`. Apply after `authGuard`, e.g.:
 *
 * ```ts
 * {
 *   path: 'logs',
 *   canActivate: [authGuard, roleGuard],
 *   data: { roles: [Roles.Administrator] } satisfies RoleRouteData,
 *   loadChildren: () => ...,
 * }
 * ```
 *
 * This is a UX concern only — the backend remains the source of truth and
 * still rejects unauthorized requests with 401/403.
 */
export const roleGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const allowedRoles = (route.data['roles'] as readonly Role[] | undefined) ?? [];

  return authService.hasRole(...allowedRoles) ? true : router.parseUrl('/home');
};
