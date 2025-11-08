import { Component, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ClientDto, CreateUserDto, UserDto } from '../models';
import { AdminService } from '../admin.service';

@Component({
  selector: 'app-admin-user-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  // Используем тот же SCSS, что и ClientList
  styleUrl: '../admin-client-list/admin-client-list.component.scss',
  templateUrl: './admin-user-list.component.html',
})
export class AdminUserListComponent implements OnInit {
  private adminService = inject(AdminService);
  private fb = inject(FormBuilder);

  users: WritableSignal<UserDto[]> = signal([]);
  clients: WritableSignal<ClientDto[]> = signal([]); // Для <select>
  isLoading = signal(true);
  error = signal<string | null>(null);

  /**
   * Форма для создания нового пользователя
   */
  userForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['Password123!', [Validators.required, Validators.minLength(6)]],
    fullName: ['', Validators.required],
    clientId: [null as number | null],
    role: ['Client', Validators.required],
  });

  ngOnInit() {
    this.loadData();
  }

  /**
   * Загружает данные (Пользователей и Клиентов) с сервера
   */
  loadData() {
    this.isLoading.set(true);
    // Загружаем одновременно и пользователей, и клиентов
    Promise.all([
      this.adminService.getUsers().toPromise(),
      this.adminService.getClients().toPromise(),
    ])
      .then(([usersData, clientsData]) => {
        this.users.set(usersData ?? []);
        this.clients.set(clientsData ?? []);
        this.isLoading.set(false);
      })
      .catch((err) => {
        this.error.set('Не удалось загрузить данные: ' + err.message);
        this.isLoading.set(false);
      });
  }

  /**
   * Вызывается при отправке формы создания пользователя
   */
  onSubmit() {
    if (this.userForm.invalid) {
      this.error.set('Форма заполнена некорректно.');
      return;
    }
    this.error.set(null);

    const formVal = this.userForm.value;

    const dto: CreateUserDto = {
      email: formVal.email!,
      password: formVal.password!,
      fullName: formVal.fullName!,
      clientId: formVal.clientId ? Number(formVal.clientId) : undefined,
      role: formVal.role!,
    };

    this.adminService.createUser(dto).subscribe({
      next: (newUser) => {
        this.users.update((list) => [newUser, ...list]);
        this.userForm.reset({
          role: 'Client',
          password: 'Password123!',
        });
      },
      error: (err) => {
        // err.error?.message - это сообщение от .NET (напр. "Пользователь уже существует")
        const errorMsg =
          err.error?.message || err.message || 'Неизвестная ошибка при создании пользователя';
        this.error.set(errorMsg);
      },
    });
  }
}
