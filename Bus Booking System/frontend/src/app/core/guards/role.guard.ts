import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { UserRole } from '../models';

function getAllowedRoles(route: ActivatedRouteSnapshot): UserRole[] {
  return (route.data['roles'] as UserRole[]) ?? [];
}

export const roleGuard: CanActivateFn = (route) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const allowedRoles = getAllowedRoles(route);

  if (authService.isLoggedIn() && authService.hasRole(...allowedRoles)) {
    return true;
  }

  return router.createUrlTree(['/']);
};
