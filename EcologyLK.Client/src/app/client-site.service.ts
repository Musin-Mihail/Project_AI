import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ClientSiteDto, CreateClientSiteDto } from './models';

/**
 * Сервис для инкапсуляции всех HTTP-запросов
 * к ClientSitesController в .NET API.
 */
@Injectable({
  providedIn: 'root',
})
export class ClientSiteService {
  private http = inject(HttpClient);

  // URL .NET API
  private apiUrl = 'https://localhost:7166/api/ClientSites';

  /**
   * POST: api/ClientSites
   * Создает новую площадку ("Анкета")
   * и возвращает созданную площадку с картой требований.
   * @param siteData DTO "Анкеты"
   * @returns Observable с DTO созданной площадки
   */
  createClientSite(siteData: CreateClientSiteDto): Observable<ClientSiteDto> {
    return this.http.post<ClientSiteDto>(this.apiUrl, siteData);
  }

  /**
   * GET: api/ClientSites/{id}
   * Получает площадку клиента по ID,
   * включая "Карту требований".
   * @param id ID площадки
   * @returns Observable с DTO площадки
   */
  getClientSite(id: number): Observable<ClientSiteDto> {
    return this.http.get<ClientSiteDto>(`${this.apiUrl}/${id}`);
  }
}
