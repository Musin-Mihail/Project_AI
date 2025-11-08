import { Component, computed, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CalendarService } from '../calendar.service';
import { CalendarEventDto } from '../models';
// --- НОВЫЕ ИМПОРТЫ ДЛЯ FULLCALENDAR ---
import { FullCalendarModule } from '@fullcalendar/angular';
import { CalendarOptions, EventInput } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import listPlugin from '@fullcalendar/list';
import interactionPlugin from '@fullcalendar/interaction';
import ruLocale from '@fullcalendar/core/locales/ru'; // Импорт русской локализации
import { map } from 'rxjs';

@Component({
  selector: 'app-calendar-view',
  standalone: true,
  imports: [CommonModule, FullCalendarModule], // (1) Добавлен FullCalendarModule
  templateUrl: './calendar-view.component.html',
  styleUrl: './calendar-view.component.scss',
})
export class CalendarViewComponent implements OnInit {
  private calendarService = inject(CalendarService);

  // (2) events сигнал теперь хранит EventInput[] для FullCalendar
  events: WritableSignal<EventInput[]> = signal([]);
  isLoading = signal(true);
  error = signal<string | null>(null);

  // (3) Опции для FullCalendar
  calendarOptions = computed<CalendarOptions>(() => ({
    plugins: [dayGridPlugin, listPlugin, interactionPlugin],
    headerToolbar: {
      left: 'prev,next today',
      center: 'title',
      right: 'dayGridMonth,listWeek',
    },
    initialView: 'dayGridMonth',
    weekends: true,
    editable: false,
    selectable: true,
    selectMirror: true,
    dayMaxEvents: true,
    locale: ruLocale, // (4) Применяем русскую локализацию
    events: this.events(), // (5) Привязываем к сигналу
  }));

  ngOnInit() {
    this.loadEvents();
  }

  loadEvents() {
    this.isLoading.set(true);
    this.calendarService
      .getCalendarEvents()
      .pipe(
        // (6) Трансформируем DTO в EventInput[]
        map((dtos: CalendarEventDto[]): EventInput[] => {
          return dtos.map((dto) => ({
            id: dto.id.toString(),
            title: `[${dto.relatedSiteName ?? 'N/A'}] - ${dto.title}`,
            start: dto.startDate,
            end: dto.endDate,
            color: dto.color,
            extendedProps: {
              siteId: dto.relatedSiteId,
            },
          }));
        })
      )
      .subscribe({
        next: (mappedEvents) => {
          this.events.set(mappedEvents);
          this.isLoading.set(false);
        },
        error: (err) => {
          this.error.set('Не удалось загрузить события: ' + err.message);
          this.isLoading.set(false);
        },
      });
  }
}
