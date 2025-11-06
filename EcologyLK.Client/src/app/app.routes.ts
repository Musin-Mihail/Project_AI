import { Routes } from '@angular/router';
import { ClientSiteFormComponent } from './client-site-form/client-site-form.component';
import { RequirementMapComponent } from './requirement-map/requirement-map.component';
import { CalendarViewComponent } from './calendar-view/calendar-view.component';

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
  // Маршрут для "Календаря"
  {
    path: 'calendar',
    component: CalendarViewComponent,
  },
  // По умолчанию перенаправляем на "Анкету"
  {
    path: '',
    redirectTo: '/new-site',
    pathMatch: 'full',
  },
];
