import { Component, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ClientSiteDto, RequirementStatus } from '../models';
import { ClientSiteService } from '../client-site.service';
import { ArtifactManagerComponent } from '../artifact-manager/artifact-manager.component';
import { FinancialDocumentListComponent } from '../financial-document-list/financial-document-list.component';

@Component({
  selector: 'app-requirement-map',
  standalone: true,
  imports: [CommonModule, ArtifactManagerComponent, FinancialDocumentListComponent],
  templateUrl: './requirement-map.component.html',
  styleUrl: './requirement-map.component.scss',
})
export class RequirementMapComponent implements OnInit {
  // Внедряем сервисы
  private route = inject(ActivatedRoute);
  private siteService = inject(ClientSiteService);

  // Используем signal для хранения состояния
  site: WritableSignal<ClientSiteDto | null> = signal(null);
  siteIdSignal = signal<number>(0);
  isLoading = signal(true);
  error = signal<string | null>(null);

  // Enum для использования в шаблоне
  RequirementStatus = RequirementStatus;

  ngOnInit() {
    // 1. Получаем 'id' из URL (/:id)
    const idParam = this.route.snapshot.paramMap.get('id');
    const siteId = Number(idParam);

    this.siteIdSignal.set(siteId);

    if (isNaN(siteId) || siteId <= 0) {
      this.error.set('Некорректный ID площадки.');
      this.isLoading.set(false);
      return;
    }

    // 2. Вызываем сервис для загрузки данных
    this.siteService.getClientSite(siteId).subscribe({
      next: (data) => {
        this.site.set(data); // 3. Сохраняем данные в signal
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Ошибка при загрузке карты требований:', err);
        this.error.set('Не удалось загрузить данные. ' + err.message);
        this.isLoading.set(false);
      },
    });
  }

  // Вспомогательная функция для отображения статуса
  getStatusText(status: RequirementStatus): string {
    switch (status) {
      case RequirementStatus.NotStarted:
        return 'Не выполнено';
      case RequirementStatus.InProgress:
        return 'В работе';
      case RequirementStatus.Completed:
        return 'Выполнено';
      default:
        return 'Неизвестно';
    }
  }
}
