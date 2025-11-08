import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UpdateRequirementDto } from './models';

/**
 * Сервис для инкапсуляции HTTP-запросов
 * к EcologicalRequirementsController в .NET API.
 */
@Injectable({
  providedIn: 'root',
})
export class RequirementService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7166/api/EcologicalRequirements';

  /**
   * PUT: api/EcologicalRequirements/{id}
   * Обновляет существующее требование (Статус, Срок, Ответственный).
   * (Admin only)
   * @param id ID требования для обновления
   * @param dto DTO с данными для обновления
   * @returns Observable<void>
   */
  updateRequirement(id: number, dto: UpdateRequirementDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
  }
}
