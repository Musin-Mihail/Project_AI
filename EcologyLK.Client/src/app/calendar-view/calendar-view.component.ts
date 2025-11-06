import { Component, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CalendarService } from '../calendar.service';
import { CalendarEventDto } from '../models';

@Component({
  selector: 'app-calendar-view',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './calendar-view.component.html',
  styleUrl: './calendar-view.component.scss',
})
export class CalendarViewComponent implements OnInit {
  private calendarService = inject(CalendarService);

  events: WritableSignal<CalendarEventDto[]> = signal([]);
  isLoading = signal(true);
  error = signal<string | null>(null);

  ngOnInit() {
    this.loadEvents();
  }

  loadEvents() {
    this.isLoading.set(true);
    this.calendarService.getCalendarEvents().subscribe({
      next: (data) => {
        this.events.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set('Не удалось загрузить события: ' + err.message);
        this.isLoading.set(false);
      },
    });
  }
}
