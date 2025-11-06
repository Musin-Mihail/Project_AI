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

  // URL нашего .NET API (из D:\Repositories!\EcologyLK.Api\Properties\launchSettings.json)
  private apiUrl = 'https://localhost:7166/api/ClientSites';

  /**
   * POST: Создает новую площадку ("Анкета")
   * и возвращает созданную площадку с картой требований.
   */
  createClientSite(siteData: CreateClientSiteDto): Observable<ClientSiteDto> {
    return this.http.post<ClientSiteDto>(this.apiUrl, siteData);
  }

  /**
   * GET: Получает площадку клиента по ID,
   * включая "Карту требований".
   */
  getClientSite(id: number): Observable<ClientSiteDto> {
    return this.http.get<ClientSiteDto>(`${this.apiUrl}/${id}`);
  }
}
