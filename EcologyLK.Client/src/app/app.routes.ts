import { Routes } from '@angular/router';
import { ClientSiteFormComponent } from './client-site-form/client-site-form.component';
import { RequirementMapComponent } from './requirement-map/requirement-map.component';
import { CalendarViewComponent } from './calendar-view/calendar-view.component';
import { LoginComponent } from './login/login.component';
import { authGuard } from './auth.guard';
import { adminGuard } from './admin.guard'; // (1) Импортируем новый guard
import { AdminUserListComponent } from './admin-user-list/admin-user-list.component'; // (2) Импорт
import { AdminClientListComponent } from './admin-client-list/admin-client-list.component'; // (3) Импорт
import { AdminLegalActListComponent } from './admin-legal-act-list/admin-legal-act-list.component'; // (4) НОВЫЙ ИМПОРТ

export const routes: Routes = [
  // Публичный маршрут для Входа
  {
    path: 'login',
    component: LoginComponent,
  },

  // --- Админ-панель (Новое) ---
  {
    path: 'admin/users',
    component: AdminUserListComponent,
    canActivate: [authGuard, adminGuard],
  },
  {
    path: 'admin/clients',
    component: AdminClientListComponent,
    canActivate: [authGuard, adminGuard],
  },
  {
    path: 'admin/legal-acts', // (5) НОВЫЙ МАРШРУТ
    component: AdminLegalActListComponent,
    canActivate: [authGuard, adminGuard],
  },

  // --- Защищенные маршруты ---

  // Маршрут для "Анкеты"
  {
    path: 'new-site',
    component: ClientSiteFormComponent,
    canActivate: [authGuard], // Защищаем
  },
  // Маршрут для "Карты требований" (принимает ID площадки)
  {
    path: 'site/:id',
    component: RequirementMapComponent,
    canActivate: [authGuard], // Защищаем
  },
  // Маршрут для "Календаря"
  {
    path: 'calendar',
    component: CalendarViewComponent,
    canActivate: [authGuard], // Защищаем
  },

  // ---

  // По умолчанию перенаправляем на "Анкету"
  // (authGuard сам перенаправит на /login, если не залогинен)
  {
    path: '',
    redirectTo: '/new-site',
    pathMatch: 'full',
  },

  // TODO: Добавить страницу 404
  {
    path: '**',
    redirectTo: '/new-site',
  },
];
