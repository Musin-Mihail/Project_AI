import { Routes } from '@angular/router';
// Импортируем новые компоненты (которые мы сейчас создадим)
import { ClientSiteFormComponent } from './client-site-form/client-site-form.component';
import { RequirementMapComponent } from './requirement-map/requirement-map.component';
// --- Добавлено ИИ (Этап 7) ---
import { CalendarViewComponent } from './calendar-view/calendar-view.component';
// --- Конец ---

export const routes: Routes = [
  // Маршрут для "Анкеты"
  {
    path: 'new-site',
    component: ClientSiteFormComponent,
  },
  // Маршрут для "Карты требований" (принимает ID площадки)
  {
    path: 'site/:id',
    component: RequirementMapComponent,
  },
  // --- Добавлено ИИ (Этап 7) ---
  // Маршрут для "Календаря"
  {
    path: 'calendar',
    component: CalendarViewComponent,
  },
  // --- Конец ---
  // По умолчанию перенаправляем на "Анкету"
  {
    path: '',
    redirectTo: '/new-site',
    pathMatch: 'full',
  },
];
