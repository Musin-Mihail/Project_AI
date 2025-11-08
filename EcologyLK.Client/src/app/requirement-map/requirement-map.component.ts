import { Component, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import {
  ClientSiteDto,
  EcologicalRequirementDto,
  RequirementStatus,
  UpdateRequirementDto,
} from '../models';
import { ClientSiteService } from '../client-site.service';
import { ArtifactManagerComponent } from '../artifact-manager/artifact-manager.component';
import { FinancialDocumentListComponent } from '../financial-document-list/financial-document-list.component';
// (1) --- ИМПОРТЫ ДЛЯ РЕДАКТИРОВАНИЯ ---
import { AuthService } from '../auth.service';
import { RequirementService } from '../requirement.service';
import { EditRequirementModalComponent } from '../edit-requirement-modal/edit-requirement-modal.component';

@Component({
  selector: 'app-requirement-map',
  standalone: true,
  // (2) --- ДОБАВЛЕНЫ НОВЫЕ КОМПОНЕНТЫ ---
  imports: [
    CommonModule,
    ArtifactManagerComponent,
    FinancialDocumentListComponent,
    EditRequirementModalComponent, // Модальное окно
  ],
  templateUrl: './requirement-map.component.html',
  styleUrl: './requirement-map.component.scss',
})
export class RequirementMapComponent implements OnInit {
  // Внедряем сервисы
  private route = inject(ActivatedRoute);
  private siteService = inject(ClientSiteService);
  // (3) --- ДОБАВЛЕНЫ СЕРВИСЫ ---
  public authService = inject(AuthService); // Сделан public для шаблона
  private requirementService = inject(RequirementService);

  // Используем signal для хранения состояния
  site: WritableSignal<ClientSiteDto | null> = signal(null);
  siteIdSignal = signal<number>(0);
  isLoading = signal(true);
  error = signal<string | null>(null);

  // (4) --- СИГНАЛЫ ДЛЯ МОДАЛЬНОГО ОКНА ---
  isModalOpen = signal(false);
  selectedRequirement = signal<EcologicalRequirementDto | null>(null);

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
    this.loadSiteData(siteId);
  }

  loadSiteData(siteId: number) {
    this.isLoading.set(true);
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

  // (5) --- МЕТОДЫ ДЛЯ РЕДАКТИРОВАНИЯ ---

  /**
   * Открывает модальное окно для редактирования
   */
  onEdit(requirement: EcologicalRequirementDto) {
    this.selectedRequirement.set(requirement);
    this.isModalOpen.set(true);
  }

  /**
   * Закрывает модальное окно
   */
  onCloseModal() {
    this.isModalOpen.set(false);
    this.selectedRequirement.set(null);
  }

  /**
   * Вызывается при сохранении из модального окна
   */
  onSaveRequirement(dto: UpdateRequirementDto) {
    const reqToUpdate = this.selectedRequirement();
    if (!reqToUpdate) return;

    this.requirementService.updateRequirement(reqToUpdate.id, dto).subscribe({
      next: () => {
        // Обновляем данные локально БЕЗ перезагрузки страницы
        this.site.update((currentSite) => {
          if (!currentSite) return null;

          const updatedRequirements = currentSite.requirements.map((req) => {
            if (req.id === reqToUpdate.id) {
              // Возвращаем обновленное требование
              return {
                ...req,
                status: dto.status,
                deadline: dto.deadline,
                responsiblePerson: dto.responsiblePerson,
              };
            }
            return req;
          });

          return { ...currentSite, requirements: updatedRequirements };
        });

        this.onCloseModal(); // Закрываем окно
      },
      error: (err) => {
        console.error('Ошибка при обновлении требования:', err);
        this.error.set('Ошибка сохранения: ' + err.message);
        // Не закрываем окно, чтобы пользователь видел ошибку
      },
    });
  }
}
