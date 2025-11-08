import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LegalActDto, CreateOrUpdateLegalActDto } from './models';

/**
 * Сервис для инкапсуляции всех HTTP-запросов
 * к LegalActsController в .NET API.
 */
@Injectable({
  providedIn: 'root',
})
export class LegalActService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7166/api/LegalActs';

  /**
   * GET: api/LegalActs
   * Получает список всех НПА. (Доступно всем аутентифицированным)
   * @returns Observable со списком DTO НПА
   */
  getLegalActs(): Observable<LegalActDto[]> {
    return this.http.get<LegalActDto[]>(this.apiUrl);
  }

  /**
   * POST: api/LegalActs
   * Создает новый НПА. (Admin only)
   * @param dto DTO для создания НПА
   * @returns Observable с DTO созданного НПА
   */
  createLegalAct(dto: CreateOrUpdateLegalActDto): Observable<LegalActDto> {
    return this.http.post<LegalActDto>(this.apiUrl, dto);
  }

  /**
   * PUT: api/LegalActs/{id}
   * Обновляет существующий НПА. (Admin only)
   * @param id ID НПА для обновления
   * @param dto DTO с данными для обновления
   * @returns Observable<void>
   */
  updateLegalAct(id: number, dto: CreateOrUpdateLegalActDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
  }

  /**
   * DELETE: api/LegalActs/{id}
   * Удаляет НПА. (Admin only)
   * @param id ID НПА для удаления
   * @returns Observable<void>
   */
  deleteLegalAct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
