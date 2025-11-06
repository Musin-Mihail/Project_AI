import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

/**
 * Admin Guard (Защитник маршрутов)
 * Запрещает доступ к страницам,
 * если пользователь не аутентифицирован ИЛИ не является 'Admin'.
 */
export const adminGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn() && authService.hasRole('Admin')) {
    return true; // Доступ разрешен
  }

  // Если залогинен, но не админ - на главную
  if (authService.isLoggedIn()) {
    console.warn('Доступ только для Администраторов.');
    router.navigate(['/new-site']);
    return false;
  }

  // Если не залогинен - на /login
  router.navigate(['/login']);
  return false;
};
