import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Route activation guard that prevents unauthenticated access to participant-only views
 * and redirects unauthorized visitors to the login page with a returnUrl query parameter.
 */
export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    return true;
  }

  // Preserve the intended target URL for post-login redirection
  router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
  return false;
};
