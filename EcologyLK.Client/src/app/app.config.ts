import {
  ApplicationConfig,
  provideBrowserGlobalErrorListeners,
  provideZoneChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';
// import { provideReactiveForms } from '@angular/forms'; // <-- Удалено ИИ

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(), // <-- Добавлено ИИ: Для выполнения HTTP-запросов к API
    // provideReactiveForms(), // <-- Удалено ИИ: Эта строка вызывала ошибку TS2305
  ],
};
