import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ArtifactDto } from './models';

/**
 * Сервис для инкапсуляции всех HTTP-запросов
 * к ArtifactsController в .NET API.
 */
@Injectable({
  providedIn: 'root',
})
export class ArtifactService {
  private http = inject(HttpClient);

  // URL .NET API
  private apiUrl = 'https://localhost:7166/api/Artifacts';

  /**
   * GET: api/Artifacts
   * Получает список артефактов (метаданные) для указанной площадки.
   * @param siteId ID площадки
   * @returns Observable со списком DTO Артефактов
   */
  getArtifactsForSite(siteId: number): Observable<ArtifactDto[]> {
    const params = new HttpParams().set('siteId', siteId.toString());
    return this.http.get<ArtifactDto[]>(this.apiUrl, { params });
  }

  /**
   * POST: api/Artifacts/Upload
   * Загружает новый файл (артефакт) для площадки.
   * @param siteId ID площадки
   * @param requirementId (Опционально) ID требования
   * @param file Загружаемый файл
   * @returns Observable с DTO созданного Артефакта
   */
  uploadArtifact(
    siteId: number,
    requirementId: number | null,
    file: File
  ): Observable<ArtifactDto> {
    const formData = new FormData();
    formData.append('file', file, file.name);

    let params = new HttpParams().set('siteId', siteId.toString());
    if (requirementId) {
      params = params.set('requirementId', requirementId.toString());
    }

    return this.http.post<ArtifactDto>(`${this.apiUrl}/Upload`, formData, {
      params,
    });
  }

  /**
   * DELETE: api/Artifacts/{id}
   * Удаляет артефакт (запись из БД и файл из хранилища).
   * @param id ID артефакта
   * @returns Observable<void>
   */
  deleteArtifact(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * GET: api/Artifacts/Download/{id}
   * Скачивает физический файл артефакта по его ID.
   * Обрабатывает ответ как 'blob' и инициирует скачивание в браузере.
   * @param id ID артефакта
   * @param fileName Имя файла, которое будет у пользователя (OriginalFileName)
   */
  downloadArtifact(id: number, fileName: string): void {
    this.http.get(`${this.apiUrl}/Download/${id}`, { responseType: 'blob' }).subscribe({
      next: (blob) => {
        // Создаем временную ссылку и "кликаем" по ней,
        // чтобы инициировать скачивание файла
        const a = document.createElement('a');
        const objectUrl = URL.createObjectURL(blob);
        a.href = objectUrl;
        a.download = fileName;
        document.body.appendChild(a); // Необходимо для Firefox
        a.click();
        URL.revokeObjectURL(objectUrl);
        document.body.removeChild(a);
      },
      error: (err) => {
        console.error('Ошибка при скачивании файла:', err);
        // Ошибка будет обработана глобально или в компоненте
      },
    });
  }
}
