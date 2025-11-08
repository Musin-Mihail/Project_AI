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
  private apiUrl = 'https://localhost:7166/api/CalendarEvents';

  /**
   * GET: api/CalendarEvents
   * Получает список всех событий (требований с Deadline)
   * с учетом RLS (Row-Level Security) пользователя.
   * @returns Observable со списком DTO событий календаря
   */
  getCalendarEvents(): Observable<CalendarEventDto[]> {
    return this.http.get<CalendarEventDto[]>(this.apiUrl);
  }
}
