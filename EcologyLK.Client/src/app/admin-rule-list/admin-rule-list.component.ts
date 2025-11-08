import { Component, computed, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CreateOrUpdateRuleDto, RequirementRuleDto, WaterUseType } from '../models';
import { RequirementRuleService } from '../requirement-rule.service';

@Component({
  selector: 'app-admin-rule-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  // Используем общие стили админ-панели
  styleUrl: '../admin-client-list/admin-client-list.component.scss',
  templateUrl: './admin-rule-list.component.html',
})
export class AdminRuleListComponent implements OnInit {
  private ruleService = inject(RequirementRuleService);
  private fb = inject(FormBuilder);

  rules: WritableSignal<RequirementRuleDto[]> = signal([]);
  isLoading = signal(true);
  error = signal<string | null>(null);

  /**
   * ID правила, которое сейчас редактируется (null = создание нового)
   */
  editingRuleId = signal<number | null>(null);

  /**
   * Опции для выпадающего списка (select) WaterUseType
   */
  waterUseOptions = Object.keys(WaterUseType)
    .filter((v) => isNaN(Number(v)) && v !== 'Other') // WaterUseType.Other не используется
    .map((key) => ({
      key: key,
      value: WaterUseType[key as keyof typeof WaterUseType],
    }));

  /**
   * Опции для выпадающего списка (select) nullable booleans
   */
  boolTriggerOptions = [
    { text: 'Не применять', value: 'null' },
    { text: 'Требуется (True)', value: 'true' },
    { text: 'Не требуется (False)', value: 'false' },
  ];

  /**
   * Форма для создания/редактирования правила
   */
  ruleForm = this.fb.group({
    description: ['', [Validators.required, Validators.maxLength(500)]],
    // --- Триггеры ---
    triggerNvosCategoryI: ['null'],
    triggerNvosCategoryII: ['null'],
    triggerNvosCategoryIII: ['null'],
    triggerNvosCategoryIV: ['null'],
    triggerWaterUseType: ['null'],
    triggerHasByproducts: ['null'],
    // --- Результат ---
    generatedTitle: ['', [Validators.required, Validators.maxLength(500)]],
    generatedBasis: ['', [Validators.required, Validators.maxLength(500)]],
    generatedPenaltyRisk: ['', [Validators.maxLength(500)]],
    isActive: [true, Validators.required],
  });

  /**
   * Вычисляемый сигнал для заголовка формы
   */
  formTitle = computed(() =>
    this.editingRuleId()
      ? `Редактирование правила ID: ${this.editingRuleId()}`
      : 'Создать новое правило'
  );

  ngOnInit() {
    this.loadRules();
  }

  /**
   * Загружает список правил генерации с сервера
   */
  loadRules() {
    this.isLoading.set(true);
    this.ruleService.getRules().subscribe({
      next: (data) => {
        this.rules.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set('Не удалось загрузить список правил: ' + err.message);
        this.isLoading.set(false);
      },
    });
  }

  /**
   * Конвертирует bool? | undefined в строку 'true'|'false'|'null'
   */
  private boolToString(val: boolean | null | undefined): string {
    if (val === true) return 'true';
    if (val === false) return 'false';
    return 'null';
  }

  /**
   * Конвертирует строку 'true'|'false'|'null' в bool?
   */
  private stringToBool(val: string | null | undefined): boolean | undefined {
    if (val === 'true') return true;
    if (val === 'false') return false;
    return undefined; // undefined в DTO = null в C#
  }

  /**
   * Конвертирует WaterUseType? | undefined в строку
   */
  private waterToString(val: WaterUseType | null | undefined): string {
    if (val === null || val === undefined) return 'null';
    return val.toString();
  }

  /**
   * Конвертирует строку в WaterUseType?
   */
  private stringToWater(val: string | null | undefined): WaterUseType | undefined {
    if (val === 'null' || val === null || val === undefined) return undefined;
    return Number(val);
  }

  /**
   * Заполняет форму для редактирования
   * @param rule DTO правила для редактирования
   */
  onEdit(rule: RequirementRuleDto) {
    this.editingRuleId.set(rule.id);
    this.ruleForm.patchValue({
      description: rule.description,
      triggerNvosCategoryI: this.boolToString(rule.triggerNvosCategoryI),
      triggerNvosCategoryII: this.boolToString(rule.triggerNvosCategoryII),
      triggerNvosCategoryIII: this.boolToString(rule.triggerNvosCategoryIII),
      triggerNvosCategoryIV: this.boolToString(rule.triggerNvosCategoryIV),
      triggerWaterUseType: this.waterToString(rule.triggerWaterUseType),
      triggerHasByproducts: this.boolToString(rule.triggerHasByproducts),
      generatedTitle: rule.generatedTitle,
      generatedBasis: rule.generatedBasis,
      generatedPenaltyRisk: rule.generatedPenaltyRisk,
      isActive: rule.isActive,
    });
  }

  /**
   * Сбрасывает форму в режим "Создание"
   */
  onCancelEdit() {
    this.editingRuleId.set(null);
    this.ruleForm.reset({
      isActive: true,
      triggerNvosCategoryI: 'null',
      triggerNvosCategoryII: 'null',
      triggerNvosCategoryIII: 'null',
      triggerNvosCategoryIV: 'null',
      triggerWaterUseType: 'null',
      triggerHasByproducts: 'null',
    });
  }

  /**
   * Отправка формы (POST или PUT)
   */
  onSubmit() {
    if (this.ruleForm.invalid) {
      this.error.set('Форма заполнена некорректно.');
      return;
    }
    this.error.set(null);
    const formVal = this.ruleForm.value;

    const dto: CreateOrUpdateRuleDto = {
      description: formVal.description!,
      triggerNvosCategoryI: this.stringToBool(formVal.triggerNvosCategoryI),
      triggerNvosCategoryII: this.stringToBool(formVal.triggerNvosCategoryII),
      triggerNvosCategoryIII: this.stringToBool(formVal.triggerNvosCategoryIII),
      triggerNvosCategoryIV: this.stringToBool(formVal.triggerNvosCategoryIV),
      triggerWaterUseType: this.stringToWater(formVal.triggerWaterUseType),
      triggerHasByproducts: this.stringToBool(formVal.triggerHasByproducts),
      generatedTitle: formVal.generatedTitle!,
      generatedBasis: formVal.generatedBasis!,
      generatedPenaltyRisk: formVal.generatedPenaltyRisk || undefined,
      isActive: formVal.isActive ?? true,
    };

    const idToUpdate = this.editingRuleId();

    if (idToUpdate) {
      // --- РЕЖИМ ОБНОВЛЕНИЯ (PUT) ---
      this.ruleService.updateRule(idToUpdate, dto).subscribe({
        next: () => {
          // Обновляем правило в локальном списке
          this.rules.update((list) =>
            list.map((r) => (r.id === idToUpdate ? { ...r, ...dto, id: idToUpdate } : r))
          );
          this.onCancelEdit();
        },
        error: (err) => {
          this.error.set('Ошибка при обновлении правила: ' + err.message);
        },
      });
    } else {
      // --- РЕЖИМ СОЗДАНИЯ (POST) ---
      this.ruleService.createRule(dto).subscribe({
        next: (newRule) => {
          this.rules.update((list) => [...list, newRule]); // Добавляем в конец
          this.onCancelEdit();
        },
        error: (err) => {
          this.error.set('Ошибка при создании правила: ' + err.message);
        },
      });
    }
  }

  /**
   * Удаление правила
   * @param rule DTO правила для удаления
   */
  onDelete(rule: RequirementRuleDto) {
    if (!confirm(`Вы уверены, что хотите удалить правило ID: ${rule.id} (${rule.description})?`)) {
      return;
    }

    this.ruleService.deleteRule(rule.id).subscribe({
      next: () => {
        this.rules.update((list) => list.filter((r) => r.id !== rule.id));
      },
      error: (err) => {
        this.error.set('Ошибка при удалении правила: ' + err.message);
      },
    });
  }
}
