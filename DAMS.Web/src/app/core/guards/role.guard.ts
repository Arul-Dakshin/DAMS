import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { Role } from '../models/auth.models';

export const roleGuard = (...roles: Role[]): CanActivateFn => {
  return () => {
    const auth = inject(AuthService);
    const router = inject(Router);
    if (auth.isAuthenticated() && auth.hasRole(...roles)) {
      return true;
    }
    router.navigate([auth.isAuthenticated() ? '/dashboard' : '/login']);
    return false;
  };
};
