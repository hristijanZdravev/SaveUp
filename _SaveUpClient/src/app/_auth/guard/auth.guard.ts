import { CanActivateFn } from '@angular/router';
import { AuthService } from '../auth.service';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {
  const authenticationService = inject(AuthService);

  if (authenticationService.isLoggedIn()) {
    return true;
  }
  authenticationService.login();
  
  return false;
};
