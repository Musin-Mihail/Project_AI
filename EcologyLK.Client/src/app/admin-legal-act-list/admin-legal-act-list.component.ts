import { Component, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { LegalActDto, CreateOrUpdateLegalActDto } from '../models';
import { LegalActService } from '../legal-act.service';

@Component({
  selector: 'app-admin-legal-act-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  // Используем общие стили админ-панели
  styleUrl: '../admin-client-list/admin-client-list.component.scss',
  templateUrl: './admin-legal-act-list.component.html',
})
export class AdminLegalActListComponent implements OnInit {
  private legalActService = inject(LegalActService);
  private fb = inject(FormBuilder);

  legalActs: WritableSignal<LegalActDto[]> = signal([]);
  isLoading = signal(true);
  error = signal<string | null>(null);

  /**
   * Форма для создания/редактирования НПА
   */
  actForm = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(500)]],
    referenceCode: ['', [Validators.required, Validators.maxLength(100)]],
    description: ['', [Validators.maxLength(1000)]],
    externalLink: ['', [Validators.maxLength(500)]], // TODO (Refactoring): Add URL validator
  });

  ngOnInit() {
    this.loadActs();
  }

  /**
   * Загружает список НПА с сервера
   */
  loadActs() {
    this.isLoading.set(true);
    this.legalActService.getLegalActs().subscribe({
      next: (data) => {
        this.legalActs.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set('Не удалось загрузить список НПА: ' + err.message);
        this.isLoading.set(false);
      },
    });
  }

  /**
   * Вызывается при отправке формы (создание или редактирование)
   */
  onSubmit() {
    if (this.actForm.invalid) {
      this.error.set('Форма заполнена некорректно.');
      return;
    }
    this.error.set(null);

    const dto: CreateOrUpdateLegalActDto = {
      title: this.actForm.value.title!,
      referenceCode: this.actForm.value.referenceCode!,
      description: this.actForm.value.description || undefined,
      externalLink: this.actForm.value.externalLink || undefined,
    };

    // TODO (Refactoring): Логика редактирования (PUT) не реализована в этом MVP
    // Удален устаревший комментарий // TODO: Добавить логику редактирования (PUT)

    this.legalActService.createLegalAct(dto).subscribe({
      next: (newAct) => {
        this.legalActs.update((list) => [...list, newAct]); // Добавляем в конец
        this.actForm.reset();
      },
      error: (err) => {
        this.error.set('Ошибка при создании НПА: ' + err.message);
      },
    });
  }

  /**
   * Вызывается при нажатии кнопки "Удалить"
   * @param act DTO НПА для удаления
   */
  onDelete(act: LegalActDto) {
    if (!confirm(`Вы уверены, что хотите удалить ${act.referenceCode}?`)) {
      return;
    }

    this.legalActService.deleteLegalAct(act.id).subscribe({
      next: () => {
        this.legalActs.update((list) => list.filter((a) => a.id !== act.id));
      },
      error: (err) => {
        this.error.set('Ошибка при удалении НПА: ' + err.message);
      },
    });
  }
}
