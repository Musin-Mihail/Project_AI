import { Component, inject, Input, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ArtifactDto } from '../models';
import { ArtifactService } from '../artifact.service';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-artifact-manager',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './artifact-manager.component.html',
  styleUrl: './artifact-manager.component.scss',
})
export class ArtifactManagerComponent implements OnInit {
  @Input({ required: true }) siteId!: number;

  private artifactService = inject(ArtifactService);
  private authService = inject(AuthService);

  artifacts: WritableSignal<ArtifactDto[]> = signal([]);
  isLoading = signal(true);
  error = signal<string | null>(null);
  fileToUpload: File | null = null;

  canDelete: boolean = false;

  ngOnInit() {
    if (this.siteId > 0) {
      this.loadArtifacts();
    }
    this.canDelete = this.authService.hasRole('Admin');
  }

  /**
   * Загружает список артефактов для площадки
   */
  loadArtifacts() {
    this.isLoading.set(true);
    this.artifactService.getArtifactsForSite(this.siteId).subscribe({
      next: (data) => {
        this.artifacts.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set('Не удалось загрузить артефакты: ' + err.message);
        this.isLoading.set(false);
      },
    });
  }

  /**
   * Вызывается при выборе файла в <input>
   */
  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.fileToUpload = input.files[0];
    }
  }

  /**
   * Вызывается при клике на "Загрузить"
   */
  onUpload() {
    if (!this.fileToUpload) {
      alert('Пожалуйста, выберите файл для загрузки.');
      return;
    }

    // TODO: Добавить выбор EcologicalRequirementId,
    // пока отправляем null
    this.artifactService.uploadArtifact(this.siteId, null, this.fileToUpload).subscribe({
      next: (newArtifact) => {
        // Добавляем новый артефакт в начало списка
        this.artifacts.update((list) => [newArtifact, ...list]);
        this.fileToUpload = null;
        // Очищаем input
        const fileInput = document.getElementById('file-upload-input') as HTMLInputElement;
        if (fileInput) fileInput.value = '';
      },
      error: (err) => {
        this.error.set('Ошибка при загрузке файла: ' + err.message);
      },
    });
  }

  /**
   * Вызывается при клике на "Скачать"
   */
  onDownload(artifact: ArtifactDto) {
    this.artifactService.downloadArtifact(artifact.id, artifact.originalFileName);
  }

  /**
   * Вызывается при клике на "Удалить"
   */
  onDelete(artifact: ArtifactDto) {
    if (!confirm(`Вы уверены, что хотите удалить ${artifact.originalFileName}?`)) {
      return;
    }

    this.artifactService.deleteArtifact(artifact.id).subscribe({
      next: () => {
        // Удаляем артефакт из списка
        this.artifacts.update((list) => list.filter((a) => a.id !== artifact.id));
      },
      error: (err) => {
        this.error.set('Ошибка при удалении файла: ' + err.message);
      },
    });
  }

  /**
   * Вспомогательная функция для форматирования размера файла
   */
  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }
}
