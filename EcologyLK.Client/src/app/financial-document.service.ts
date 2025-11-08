import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FinancialDocumentDto } from './models';

/**
 * Сервис для инкапсуляции HTTP-запросов
 * к FinancialDocumentsController в .NET API.
 */
@Injectable({
  providedIn: 'root',
})
export class FinancialDocumentService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7166/api/FinancialDocuments';

  /**
   * GET: api/FinancialDocuments?siteId=...
   * Получает список фин. документов для указанной площадки.
   * @param siteId ID площадки, для которой запрашиваются документы
   * @returns Observable со списком DTO Финансовых документов
   */
  getDocumentsForSite(siteId: number): Observable<FinancialDocumentDto[]> {
    const params = new HttpParams().set('siteId', siteId.toString());
    return this.http.get<FinancialDocumentDto[]>(this.apiUrl, { params });
  }
}
