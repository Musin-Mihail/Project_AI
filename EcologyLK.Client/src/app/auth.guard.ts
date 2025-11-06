import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

/**
 * Auth Guard (Защитник маршрутов)
 * Запрещает доступ к защищенным страницам,
 * если пользователь не аутентифицирован.
 */
export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn()) {
    return true; // Доступ разрешен
  } else {
    // Доступ запрещен - перенаправляем на /login
    router.navigate(['/login']);
    return false;
  }
};
