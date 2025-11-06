import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';

/**
 * HTTP Interceptor (перехватчик)
 * Автоматически добавляет JWT (Bearer token)
 * ко всем исходящим HTTP-запросам
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  // Если токен есть, клонируем запрос и добавляем заголовок
  if (token) {
    const authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });
    return next(authReq);
  }

  // Если токена нет (например, запрос на Login),
  // пропускаем запрос без изменений
  return next(req);
};
