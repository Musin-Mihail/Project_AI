import { Routes } from '@angular/router';
// Импортируем новые компоненты (которые мы сейчас создадим)
import { ClientSiteFormComponent } from './client-site-form/client-site-form.component';
import { RequirementMapComponent } from './requirement-map/requirement-map.component';

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
  // По умолчанию перенаправляем на "Анкету"
  {
    path: '',
    redirectTo: '/new-site',
    pathMatch: 'full',
  },
];
