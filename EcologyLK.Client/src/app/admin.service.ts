import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ClientDto, CreateClientDto, UserDto, CreateUserDto, UpdateUserDto } from './models';

/**
 * Сервис для инкапсуляции всех HTTP-запросов
 * к AdminController в .NET API.
 */
@Injectable({
  providedIn: 'root',
})
export class AdminService {
  private http = inject(HttpClient);

  // URL .NET API
  private apiUrl = 'https://localhost:7166/api/Admin';

  // --- Управление Клиентами (ЮрЛицами) ---

  /**
   * GET: api/Admin/Clients
   * Получает список всех Клиентов (ЮрЛиц).
   * @returns Observable со списком DTO Клиентов
   */
  getClients(): Observable<ClientDto[]> {
    return this.http.get<ClientDto[]>(`${this.apiUrl}/Clients`);
  }

  /**
   * POST: api/Admin/Clients
   * Создает нового Клиента (ЮрЛицо).
   * @param dto DTO для создания клиента
   * @returns Observable с созданным DTO Клиента
   */
  createClient(dto: CreateClientDto): Observable<ClientDto> {
    return this.http.post<ClientDto>(`${this.apiUrl}/Clients`, dto);
  }

  // --- Управление Пользователями (AppUser) ---

  /**
   * GET: api/Admin/Users
   * Получает список всех Пользователей.
   * @returns Observable со списком DTO Пользователей
   */
  getUsers(): Observable<UserDto[]> {
    return this.http.get<UserDto[]>(`${this.apiUrl}/Users`);
  }

  /**
   * POST: api/Admin/Users
   * Создает нового Пользователя (Администратором).
   * @param dto DTO для создания пользователя
   * @returns Observable с созданным DTO Пользователя
   */
  createUser(dto: CreateUserDto): Observable<UserDto> {
    return this.http.post<UserDto>(`${this.apiUrl}/Users`, dto);
  }

  /**
   * PUT: api/Admin/Users/{id}
   * Обновляет данные Пользователя.
   * @param id ID пользователя (GUID)
   * @param dto DTO с данными для обновления
   * @returns Observable<void>
   */
  updateUser(id: string, dto: UpdateUserDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/Users/${id}`, dto);
  }
}
