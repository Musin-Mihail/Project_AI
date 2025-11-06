import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from './auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [CommonModule, RouterOutlet, RouterLink], // Добавлен CommonModule
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  // (1) --- ИЗМЕНЕНО: Сделано public для доступа из шаблона (app.html) ---
  public authService = inject(AuthService);
  private router = inject(Router);

  // Получаем сигнал о текущем пользователе из сервиса
  currentUser = this.authService.currentUser;

  /**
   * Выход из системы
   */
  logout() {
    this.authService.logout();
    // AuthService сам выполнит редирект на /login
  }
}
