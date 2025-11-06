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

  // URL нашего .NET API
  private apiUrl = 'https://localhost:7166/api/Admin';

  // --- Управление Клиентами (ЮрЛицами) ---

  /**
   * GET: api/Admin/Clients
   * Получает список всех Клиентов (ЮрЛиц).
   */
  getClients(): Observable<ClientDto[]> {
    return this.http.get<ClientDto[]>(`${this.apiUrl}/Clients`);
  }

  /**
   * POST: api/Admin/Clients
   * Создает нового Клиента (ЮрЛицо).
   */
  createClient(dto: CreateClientDto): Observable<ClientDto> {
    return this.http.post<ClientDto>(`${this.apiUrl}/Clients`, dto);
  }

  // --- Управление Пользователями (AppUser) ---

  /**
   * GET: api/Admin/Users
   * Получает список всех Пользователей.
   */
  getUsers(): Observable<UserDto[]> {
    return this.http.get<UserDto[]>(`${this.apiUrl}/Users`);
  }

  /**
   * POST: api/Admin/Users
   * Создает нового Пользователя (Администратором).
   */
  createUser(dto: CreateUserDto): Observable<UserDto> {
    return this.http.post<UserDto>(`${this.apiUrl}/Users`, dto);
  }

  /**
   * PUT: api/Admin/Users/{id}
   * Обновляет данные Пользователя.
   */
  updateUser(id: string, dto: UpdateUserDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/Users/${id}`, dto);
  }
}
