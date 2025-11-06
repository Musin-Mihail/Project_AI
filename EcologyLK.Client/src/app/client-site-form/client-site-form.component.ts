import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NvosCategory, WaterUseType, CreateClientSiteDto } from '../models';
import { ClientSiteService } from '../client-site.service';

@Component({
  selector: 'app-client-site-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule], // Импортируем CommonModule и ReactiveFormsModule
  templateUrl: './client-site-form.component.html',
  styleUrl: './client-site-form.component.scss',
})
export class ClientSiteFormComponent {
  // Внедряем сервисы
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private siteService = inject(ClientSiteService);

  // Опции для выпадающих списков (select)
  // Мы используем Object.keys для получения строковых ключей Enum,
  // а затем фильтруем, чтобы убрать числовые ключи, которые TypeScript создает для enums
  nvosOptions = Object.keys(NvosCategory).filter((v) => isNaN(Number(v)));
  waterUseOptions = Object.keys(WaterUseType).filter((v) => isNaN(Number(v)));

  // Определяем нашу форму
  siteForm = this.fb.group({
    // TODO: ClientId должен браться из сессии пользователя
    // Для MVP просто жестко задаем '1'
    clientId: [1, Validators.required],
    name: ['Тестовая площадка', Validators.required],
    address: ['г. Москва, ул. Пример, д. 1', Validators.required],
    // --- Исправления ИИ (Ошибки TS2352) ---
    // Инициализируем форму строковыми значениями Enum (напр. "III"),
    // а не числовыми (напр. 2), чтобы тип соответствовал <option value="III">
    nvosCategory: [NvosCategory[NvosCategory.III], Validators.required],
    waterUseType: [WaterUseType[WaterUseType.None], Validators.required],
    // --- Конец исправлений ИИ ---
    hasByproducts: [false, Validators.required],
  });

  /**
   * Вызывается при отправке формы
   */
  onSubmit() {
    if (this.siteForm.invalid) {
      console.error('Форма невалидна');
      return;
    }

    // Преобразуем значения формы в DTO
    const formValue = this.siteForm.value;
    const dto: CreateClientSiteDto = {
      clientId: formValue.clientId!,
      name: formValue.name!,
      address: formValue.address!,
      // Преобразуем строковые значения из <select> обратно в числа Enum
      // Эта логика была верной, ошибка была только в инициализации
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
