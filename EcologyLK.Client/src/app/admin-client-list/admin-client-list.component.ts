import { Component, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ClientDto, CreateClientDto } from '../models';
import { AdminService } from '../admin.service';

@Component({
  selector: 'app-admin-client-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './admin-client-list.component.html',
  styleUrl: './admin-client-list.component.scss',
})
export class AdminClientListComponent implements OnInit {
  private adminService = inject(AdminService);
  private fb = inject(FormBuilder);

  clients: WritableSignal<ClientDto[]> = signal([]);
  isLoading = signal(true);
  error = signal<string | null>(null);

  /**
   * Форма для создания нового клиента
   */
  clientForm = this.fb.group({
    name: ['', Validators.required],
    inn: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(12)]],
    ogrn: ['', [Validators.minLength(13), Validators.maxLength(15)]],
  });

  ngOnInit() {
    this.loadClients();
  }

  /**
   * Загружает список клиентов с сервера
   */
  loadClients() {
    this.isLoading.set(true);
    this.adminService.getClients().subscribe({
      next: (data) => {
        this.clients.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set('Не удалось загрузить список клиентов: ' + err.message);
        this.isLoading.set(false);
      },
    });
  }

  /**
   * Вызывается при отправке формы создания клиента
   */
  onSubmit() {
    if (this.clientForm.invalid) {
      this.error.set('Форма заполнена некорректно.');
      return;
    }
    this.error.set(null);

    const dto: CreateClientDto = {
      name: this.clientForm.value.name!,
      inn: this.clientForm.value.inn!,
      ogrn: this.clientForm.value.ogrn!,
    };

    this.adminService.createClient(dto).subscribe({
      next: (newClient) => {
        this.clients.update((list) => [newClient, ...list]);
        this.clientForm.reset();
      },
      error: (err) => {
        this.error.set('Ошибка при создании клиента: ' + err.message);
      },
    });
  }
}
