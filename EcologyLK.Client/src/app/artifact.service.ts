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

  // URL нашего .NET API (из D:\Repositories!\EcologyLK.Api\Properties\launchSettings.json)
  private apiUrl = 'https://localhost:7166/api/Artifacts';

  /**
   * GET: Получает список артефактов (метаданные) для указанной площадки.
   */
  getArtifactsForSite(siteId: number): Observable<ArtifactDto[]> {
    const params = new HttpParams().set('siteId', siteId.toString());
    return this.http.get<ArtifactDto[]>(this.apiUrl, { params });
  }

  /**
   * POST: Загружает новый файл (артефакт) для площадки.
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
   * DELETE: Удаляет артефакт (запись из БД и файл из хранилища).
   */
  deleteArtifact(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * GET: Скачивает физический файл артефакта по его ID.
   * Обрабатывает ответ как 'blob' и инициирует скачивание в браузере.
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
        // TODO: Показать ошибку пользователю
      },
    });
  }
}
