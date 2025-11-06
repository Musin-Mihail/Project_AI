import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CalendarEventDto } from './models';

/**
 * Сервис для инкапсуляции всех HTTP-запросов
 * к CalendarEventsController в .NET API.
 */
@Injectable({
  providedIn: 'root',
})
export class CalendarService {
  private http = inject(HttpClient);

  // URL нашего .NET API (из D:\Repositories!\EcologyLK.Api\Properties\launchSettings.json)
  private apiUrl = 'https://localhost:7166/api/CalendarEvents';

  /**
   * GET: Получает список всех событий
   */
  getCalendarEvents(): Observable<CalendarEventDto[]> {
    return this.http.get<CalendarEventDto[]>(this.apiUrl);
  }
}
