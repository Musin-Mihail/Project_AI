import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RequirementRuleDto, CreateOrUpdateRuleDto } from './models';

/**
 * Сервис для инкапсуляции всех HTTP-запросов
 * к RequirementRulesController в .NET API. (Admin only)
 */
@Injectable({
  providedIn: 'root',
})
export class RequirementRuleService {
  private http = inject(HttpClient);

  // URL нашего .NET API
  private apiUrl = 'https://localhost:7166/api/RequirementRules';

  /**
   * GET: api/RequirementRules
   * Получает список всех правил.
   */
  getRules(): Observable<RequirementRuleDto[]> {
    return this.http.get<RequirementRuleDto[]>(this.apiUrl);
  }

  /**
   * POST: api/RequirementRules
   * Создает новое правило.
   */
  createRule(dto: CreateOrUpdateRuleDto): Observable<RequirementRuleDto> {
    return this.http.post<RequirementRuleDto>(this.apiUrl, dto);
  }

  /**
   * PUT: api/RequirementRules/{id}
   * Обновляет существующее правило.
   */
  updateRule(id: number, dto: CreateOrUpdateRuleDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
  }

  /**
   * DELETE: api/RequirementRules/{id}
   * Удаляет правило.
   */
  deleteRule(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
