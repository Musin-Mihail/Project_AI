import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { AuthResponseDto, LoginUserDto, RegisterUserDto } from './models';
import { Router } from '@angular/router';

/**
 * Ключ для хранения сессии в localStorage
 */
const AUTH_SESSION_KEY = 'ecology_lk_session';

/**
 * Сервис для управления Аутентификацией
 * (Вход, Регистрация, Хранение токена и состояния пользователя)
 */
@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  // URL .NET Auth API
  private apiUrl = 'https://localhost:7166/api/Auth';

  /**
   * Signal, хранящий данные текущего пользователя (или null).
   * Является источником правды о состоянии аутентификации в приложении.
   */
  currentUser = signal<AuthResponseDto | null>(null);

  constructor() {
    // При инициализации сервиса, пытаемся загрузить сессию из localStorage
    this.loadSession();
  }

  /**
   * Проверяет, залогинен ли пользователь (currentUser не null).
   * @returns true, если пользователь аутентифицирован
   */
  isLoggedIn(): boolean {
    return !!this.currentUser();
  }

  /**
   * Возвращает текущий JWT (Bearer token).
   * @returns Строка с JWT или null
   */
  getToken(): string | null {
    return this.currentUser()?.token ?? null;
  }

  /**
   * Проверяет, есть ли у пользователя указанная роль
   * @param role Имя роли (напр. 'Admin')
   * @returns true, если пользователь имеет данную роль
   */
  hasRole(role: string): boolean {
    const user = this.currentUser();
    if (!user || !user.roles) {
      return false;
    }
    // Проверяем наличие роли (без учета регистра)
    return user.roles.some((r) => r.toLowerCase() === role.toLowerCase());
  }

  /**
   * POST: api/Auth/Login
   * Выполняет вход пользователя.
   * @param dto Данные для входа (Email, Пароль)
   * @returns Observable с полным DTO ответа (AuthResponseDto)
   */
  login(dto: LoginUserDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/Login`, dto).pipe(
      tap((response) => {
        // При успехе - сохраняем сессию
        this.saveSession(response);
      })
    );
  }

  /**
   * POST: api/Auth/Register
   * Регистрирует нового пользователя.
   * (В MVP не используется в UI, но доступен для Админ-панели)
   * @param dto Данные для регистрации
   * @returns Observable<any>
   */
  register(dto: RegisterUserDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/Register`, dto);
  }

  /**
   * Выполняет выход (очищает сессию и перенаправляет на /login)
   */
  logout() {
    this.clearSession();
    this.router.navigate(['/login']);
  }

  // --- Приватные методы управления сессией ---

  /**
   * Загружает сессию из localStorage при инициализации сервиса.
   */
  private loadSession() {
    try {
      const storedSession = localStorage.getItem(AUTH_SESSION_KEY);
      if (storedSession) {
        const session: AuthResponseDto = JSON.parse(storedSession);
        // TODO (в будущем): Проверить срок жизни токена
        this.currentUser.set(session);
      }
    } catch (e) {
      console.error('Не удалось загрузить сессию', e);
      this.clearSession();
    }
  }

  /**
   * Сохраняет данные пользователя в localStorage и обновляет signal.
   * @param response DTO ответа от API
   */
  private saveSession(response: AuthResponseDto) {
    try {
      localStorage.setItem(AUTH_SESSION_KEY, JSON.stringify(response));
      this.currentUser.set(response);
    } catch (e) {
      console.error('Не удалось сохранить сессию', e);
    }
  }

  /**
   * Очищает сессию из localStorage и сбрасывает signal.
   */
  private clearSession() {
    try {
      localStorage.removeItem(AUTH_SESSION_KEY);
      this.currentUser.set(null);
    } catch (e) {
      console.error('Не удалось очистить сессию', e);
    }
  }
}
