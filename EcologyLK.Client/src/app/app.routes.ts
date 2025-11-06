import { Routes } from '@angular/router';
import { ClientSiteFormComponent } from './client-site-form/client-site-form.component';
import { RequirementMapComponent } from './requirement-map/requirement-map.component';
import { CalendarViewComponent } from './calendar-view/calendar-view.component';
import { LoginComponent } from './login/login.component';
import { authGuard } from './auth.guard';

export const routes: Routes = [
  // Публичный маршрут для Входа
  {
    path: 'login',
    component: LoginComponent,
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
