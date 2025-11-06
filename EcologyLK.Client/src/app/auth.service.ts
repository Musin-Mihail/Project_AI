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
 * (Вход, Регистрация, Хранение токена)
 */
@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  // URL нашего .NET Auth API
  private apiUrl = 'https://localhost:7166/api/Auth';

  // Signal для хранения текущего пользователя
  currentUser = signal<AuthResponseDto | null>(null);

  constructor() {
    // При инициализации сервиса, пытаемся загрузить сессию из localStorage
    this.loadSession();
  }

  /**
   * Проверяет, залогинен ли пользователь
   */
  isLoggedIn(): boolean {
    return !!this.currentUser();
  }

  /**
   * Возвращает текущий JWT
   */
  getToken(): string | null {
    return this.currentUser()?.token ?? null;
  }

  /**
   * POST: api/Auth/Login
   * Выполняет вход
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
   * (Пока не используется в UI, но доступен)
   */
  register(dto: RegisterUserDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/Register`, dto);
  }

  /**
   * Выполняет выход
   */
  logout() {
    this.clearSession();
    this.router.navigate(['/login']);
  }

  // --- Приватные методы управления сессией ---

  private loadSession() {
    try {
      const storedSession = localStorage.getItem(AUTH_SESSION_KEY);
      if (storedSession) {
        const session: AuthResponseDto = JSON.parse(storedSession);
        // TODO: Проверить срок жизни токена
        this.currentUser.set(session);
      }
    } catch (e) {
      console.error('Не удалось загрузить сессию', e);
      this.clearSession();
    }
  }

  private saveSession(response: AuthResponseDto) {
    try {
      localStorage.setItem(AUTH_SESSION_KEY, JSON.stringify(response));
      this.currentUser.set(response);
    } catch (e) {
      console.error('Не удалось сохранить сессию', e);
    }
  }

  private clearSession() {
    try {
      localStorage.removeItem(AUTH_SESSION_KEY);
      this.currentUser.set(null);
    } catch (e) {
      console.error('Не удалось очистить сессию', e);
    }
  }
}
