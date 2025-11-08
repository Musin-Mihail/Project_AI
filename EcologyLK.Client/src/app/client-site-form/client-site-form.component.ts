import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NvosCategory, WaterUseType, CreateClientSiteDto } from '../models';
import { ClientSiteService } from '../client-site.service';
import { AuthService } from '../auth.service';

/**
 * Компонент "Анкеты" (CreateClientSite).
 * Отвечает за сбор данных о новой площадке и отправку их в API
 * для генерации "Карты требований".
 */
@Component({
  selector: 'app-client-site-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './client-site-form.component.html',
  styleUrl: './client-site-form.component.scss',
})
export class ClientSiteFormComponent {
  /**
   * Опции для выпадающего списка (select) "Категория НВОС".
   */
  nvosOptions = Object.keys(NvosCategory).filter((v) => isNaN(Number(v)));
  /**
   * Опции для выпадающего списка (select) "Тип водопользования".
   */
  waterUseOptions = Object.keys(WaterUseType).filter((v) => isNaN(Number(v)));

  /**
   * Реактивная форма Angular для "Анкеты".
   */
  siteForm: FormGroup;
  /**
   * Флаг, определяющий, является ли текущий пользователь Администратором.
   * (Влияет на отображение поля `clientId`).
   */
  isAdmin = false;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private siteService: ClientSiteService,
    public authService: AuthService
  ) {
    this.isAdmin = this.authService.hasRole('Admin');

    // Динамическая конфигурация формы
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

    this.siteForm = this.fb.group(formConfig);
  }

  /**
   * Вызывается при отправке формы "Анкеты".
   * Собирает данные, формирует DTO и отправляет в API.
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
      // (Refactoring Note): Здесь должна быть user-friendly ошибка
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
        // (Refactoring Note): Здесь должна быть user-friendly ошибка
      },
    });
  }
}
