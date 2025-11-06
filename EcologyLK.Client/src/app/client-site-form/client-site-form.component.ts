import { Component, inject } from '@angular/core';
// --- ИЗМЕНЕНО: Добавлен FormGroup ---
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NvosCategory, WaterUseType, CreateClientSiteDto } from '../models';
import { ClientSiteService } from '../client-site.service';
import { AuthService } from '../auth.service'; // Импортируем AuthService

@Component({
  selector: 'app-client-site-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './client-site-form.component.html',
  styleUrl: './client-site-form.component.scss',
})
export class ClientSiteFormComponent {
  // Опции для выпадающих списков (select)
  nvosOptions = Object.keys(NvosCategory).filter((v) => isNaN(Number(v)));
  waterUseOptions = Object.keys(WaterUseType).filter((v) => isNaN(Number(v)));

  // --- ИЗМЕНЕНО: Форма теперь объявляется здесь, а инициализируется в конструкторе ---
  siteForm: FormGroup;
  // --- ДОБАВЛЕНО: Флаг для UI ---
  isAdmin = false;

  // --- ИЗМЕНЕНО: Используем внедрение через конструктор ---
  constructor(
    private fb: FormBuilder,
    private router: Router,
    private siteService: ClientSiteService,
    // --- ИЗМЕНЕНО: authService сделан public для доступа из шаблона ---
    public authService: AuthService
  ) {
    // --- ДОБАВЛЕНО: Проверяем роль ---
    this.isAdmin = this.authService.hasRole('Admin');

    // --- ДОБАВЛЕНО: Динамическая конфигурация формы ---
    const formConfig: any = {
      name: ['Тестовая площадка', Validators.required],
      address: ['г. Москва, ул. Пример, д. 1', Validators.required],
      nvosCategory: [NvosCategory[NvosCategory.III], Validators.required],
      waterUseType: [WaterUseType[WaterUseType.None], Validators.required],
      hasByproducts: [false, Validators.required],
    };

    // Если пользователь - Админ, добавляем в форму поле ClientId
    if (this.isAdmin) {
      formConfig.clientId = [1, [Validators.required, Validators.min(1)]];
    }

    // --- ИЗМЕНЕНО: Инициализация формы ---
    this.siteForm = this.fb.group(formConfig);
  }

  /**
   * Вызывается при отправке формы
   */
  onSubmit() {
    if (this.siteForm.invalid) {
      console.error('Форма невалидна');
      return;
    }

    let targetClientId: number | undefined;
    const formValue = this.siteForm.value;

    if (this.isAdmin) {
      // Админ берет ClientId из формы
      targetClientId = formValue.clientId;
    } else {
      // Клиент берет ClientId из своей сессии
      targetClientId = this.authService.currentUser()?.clientId;
    }

    if (!targetClientId) {
      console.error('Ошибка: Не удалось определить ClientId для создания площадки.');
      // TODO: Показать ошибку пользователю
      return;
    }

    // Преобразуем значения формы в DTO
    const dto: CreateClientSiteDto = {
      clientId: targetClientId, // Используем определенный ClientId
      name: formValue.name!,
      address: formValue.address!,
      nvosCategory: NvosCategory[formValue.nvosCategory! as keyof typeof NvosCategory],
      waterUseType: WaterUseType[formValue.waterUseType! as keyof typeof WaterUseType],
      hasByproducts: formValue.hasByproducts!,
    };

    console.log('Отправка DTO:', dto);

    // Вызываем сервис
    this.siteService.createClientSite(dto).subscribe({
      next: (newSite) => {
        console.log('Площадка создана:', newSite);
        // При успехе - перенаправляем на страницу "Карты требований"
        // с ID новой площадки
        this.router.navigate(['/site', newSite.id]);
      },
      error: (err) => {
        console.error('Ошибка при создании площадки:', err);
        // TODO: Показать ошибку пользователю
      },
    });
  }
}
