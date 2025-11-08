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

  // URL нашего .NET API
  private apiUrl = 'https://localhost:7166/api/EcologicalRequirements';

  /**
   * PUT: api/EcologicalRequirements/{id}
   * Обновляет существующее требование.
   */
  updateRequirement(id: number, dto: UpdateRequirementDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
  }
}
